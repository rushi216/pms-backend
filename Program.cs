namespace pms_backend;

using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using pms_backend.Data;
using pms_backend.Services;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using pms_backend.Auth;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173") // Replace with your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure Microsoft Graph
        var tenantId = builder.Configuration["AzureAd:TenantId"];
        var clientId = builder.Configuration["AzureAd:ClientId"];
        var clientSecret = builder.Configuration["AzureAd:ClientSecret"];

        var credentials = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var graphClient = new GraphServiceClient(credentials);
        builder.Services.AddSingleton(graphClient);

        // Register UserSyncService
        builder.Services.AddScoped<IUserSyncService, UserSyncService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();

        // Add authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        // Add custom token validation middleware
        app.UseTokenValidation();

        app.MapControllers();

        // Run initial user sync
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var syncService = scope.ServiceProvider.GetRequiredService<IUserSyncService>();
                await syncService.SyncUsersAsync();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while syncing users during startup");
            }
        }

        await app.RunAsync();
    }
}
