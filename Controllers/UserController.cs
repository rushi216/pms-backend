using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pms_backend.Data;
using pms_backend.Models;
using Microsoft.AspNetCore.Authorization;
using pms_backend.Auth;
using Microsoft.Identity.Web;

namespace pms_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/User/me
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.GetObjectId(); // Get Azure AD Object ID from token
        
        var user = await _context.Users
            .Include(u => u.ManagerMappings)
                .ThenInclude(mm => mm.Manager)
            .Where(u => u.MicrosoftId == userId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                MicrosoftId = u.MicrosoftId,
                Name = u.Name,
                CurrentManager = u.ManagerMappings
                    .Where(mm => !mm.IsDeleted)
                    .OrderByDescending(mm => mm.AssignmentTimestamp)
                    .Select(mm => new UserManagerInfo
                    {
                        Id = mm.Manager.Id,
                        Name = mm.Manager.Name,
                        Email = mm.Manager.Email,
                        AssignmentTimestamp = mm.AssignmentTimestamp
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.ManagerMappings)
                .ThenInclude(mm => mm.Manager)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                MicrosoftId = u.MicrosoftId,
                Name = u.Name,
                CurrentManager = u.ManagerMappings
                    .Where(mm => !mm.IsDeleted)
                    .OrderByDescending(mm => mm.AssignmentTimestamp)
                    .Select(mm => new UserManagerInfo
                    {
                        Id = mm.Manager.Id,
                        Name = mm.Manager.Name,
                        Email = mm.Manager.Email,
                        AssignmentTimestamp = mm.AssignmentTimestamp
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return users;
    }

    // GET: api/User/reportees
    [HttpGet("reportees")]
    public async Task<ActionResult<ReporteesResponse>> GetReportees()
    {
        var userId = User.GetObjectId(); // Get Azure AD Object ID from token
        
        var currentUser = await _context.Users
            .FirstOrDefaultAsync(u => u.MicrosoftId == userId);

        if (currentUser == null)
        {
            return NotFound();
        }

        // Get all users who have ever been reportees of the current user
        var allReportees = await _context.Users
            .Include(u => u.ManagerMappings)
                .ThenInclude(mm => mm.Manager)
            .Where(u => u.ManagerMappings
                .Any(mm => mm.ManagerId == currentUser.Id))
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                MicrosoftId = u.MicrosoftId,
                Name = u.Name,
                CurrentManager = u.ManagerMappings
                    .Where(mm => !mm.IsDeleted)
                    .OrderByDescending(mm => mm.AssignmentTimestamp)
                    .Select(mm => new UserManagerInfo
                    {
                        Id = mm.Manager.Id,
                        Name = mm.Manager.Name,
                        Email = mm.Manager.Email,
                        AssignmentTimestamp = mm.AssignmentTimestamp
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var currentReportees = await _context.Users
            .Where(u => u.ManagerMappings
                .Any(mm => !mm.IsDeleted && mm.ManagerId == currentUser.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var response = new ReporteesResponse
        {
            CurrentReportees = allReportees.Where(r => currentReportees.Contains(r.Id)),
            PreviousReportees = allReportees.Where(r => !currentReportees.Contains(r.Id))
        };

        return response;
    }

    // POST: api/User/AssignManager
    [HttpPost("AssignManager")]
    public async Task<IActionResult> AssignManager(AssignManagerRequest request)
    {
        // Validate that both user and manager exist
        var user = await _context.Users.FindAsync(request.UserId);
        var manager = await _context.Users.FindAsync(request.ManagerId);

        if (user == null)
            return NotFound($"User with ID {request.UserId} not found.");
        
        if (manager == null)
            return NotFound($"Manager with ID {request.ManagerId} not found.");

        // Check if there's an existing active mapping
        var existingMapping = await _context.UserManagerMappings
            .Where(m => m.UserId == request.UserId && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingMapping != null)
        {
            // Soft delete the existing mapping
            existingMapping.IsDeleted = true;
        }

        // Create new mapping
        var newMapping = new UserManagerMapping
        {
            UserId = request.UserId,
            ManagerId = request.ManagerId,
            AssignmentTimestamp = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.UserManagerMappings.Add(newMapping);
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"Manager {manager.Name} successfully assigned to user {user.Name}" });
    }
}