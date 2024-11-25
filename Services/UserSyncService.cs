using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pms_backend.Data;
using pms_backend.Models;

namespace pms_backend.Services;

public interface IUserSyncService
{
    Task SyncUsersAsync();
}

public class UserSyncService : IUserSyncService
{
    private readonly GraphServiceClient _graphClient;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserSyncService> _logger;
    private readonly IConfiguration _configuration;

    public UserSyncService(
        GraphServiceClient graphClient,
        ApplicationDbContext context,
        ILogger<UserSyncService> logger,
        IConfiguration configuration)
    {
        _graphClient = graphClient;
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SyncUsersAsync()
    {
        try
        {
            _logger.LogInformation("Starting user synchronization from Microsoft Graph");

            // Get all users with business licenses
            var graphUsers = await GetLicensedUsersAsync();
            
            foreach (var graphUser in graphUsers)
            {
                try
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.MicrosoftId == graphUser.Id);

                    if (existingUser == null)
                    {
                        // Add new user
                        var newUser = new Models.User
                        {
                            MicrosoftId = graphUser.Id!,
                            Email = graphUser.Mail ?? graphUser.UserPrincipalName!,
                            Name = graphUser.DisplayName ?? "Unknown"
                        };

                        _context.Users.Add(newUser);
                        _logger.LogInformation("Added new user: {Email}", newUser.Email);
                    }
                    else
                    {
                        // Update existing user
                        existingUser.Email = graphUser.Mail ?? graphUser.UserPrincipalName!;
                        existingUser.Name = graphUser.DisplayName ?? "Unknown";
                        _logger.LogInformation("Updated existing user: {Email}", existingUser.Email);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing user {UserId}", graphUser.Id);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("User synchronization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user synchronization");
            throw;
        }
    }

    private async Task<IEnumerable<Microsoft.Graph.Models.User>> GetLicensedUsersAsync()
    {
        var users = new List<Microsoft.Graph.Models.User>();
        var businessLicenseSkuIds = new HashSet<string>(_configuration.GetSection("LicenseConfiguration:BusinessLicenses").Get<string[]>() ?? Array.Empty<string>());

        try
        {
            var pageSize = 100;
            var result = await _graphClient.Users.GetAsync(requestConfig =>
            {
                requestConfig.QueryParameters.Select = new[] 
                { 
                    "id",
                    "displayName",
                    "mail",
                    "userPrincipalName",
                    "assignedLicenses"
                };
                requestConfig.QueryParameters.Filter = "accountEnabled eq true";
                requestConfig.QueryParameters.Top = pageSize;
                requestConfig.Headers.Add("ConsistencyLevel", "eventual");
                requestConfig.QueryParameters.Count = true;
            });

            if (result?.Value != null)
            {
                var pageIterator = PageIterator<Microsoft.Graph.Models.User, UserCollectionResponse>
                    .CreatePageIterator(_graphClient, result, 
                    (user) =>
                    {
                        if (user.AssignedLicenses != null && 
                            user.AssignedLicenses.Any(license => 
                                license.SkuId.HasValue && 
                                businessLicenseSkuIds.Contains(license.SkuId.Value.ToString())))
                        {
                            _logger.LogInformation("Found licensed user: {DisplayName} ({UserPrincipalName})", 
                                user.DisplayName, user.UserPrincipalName);
                            users.Add(user);
                        }
                        return true;
                    });

                await pageIterator.IterateAsync();
                _logger.LogInformation("Total users found: {Count}", users.Count);
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching licensed users from Microsoft Graph");
            throw;
        }
    }
}
