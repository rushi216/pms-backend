using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pms_backend.Data;
using pms_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace pms_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Review
    [HttpPost]
    public async Task<ActionResult<ReviewResponseDto>> AddReview(ReviewDto reviewDto)
    {
        var userId = User.GetObjectId();
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == userId);
        
        if (currentUser == null)
            return NotFound("Current user not found");

        var review = new Review
        {
            ForUserId = reviewDto.UserId,
            FromUserId = currentUser.Id,
            Quarter = reviewDto.Quarter,
            Year = reviewDto.Year,
            Q1 = reviewDto.Q1,
            A1 = reviewDto.A1,
            Q2 = reviewDto.Q2,
            A2 = reviewDto.A2,
            Q3 = reviewDto.Q3,
            A3 = reviewDto.A3
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Load related users for the response
        await _context.Entry(review)
            .Reference(r => r.ForUser)
            .LoadAsync();
        await _context.Entry(review)
            .Reference(r => r.FromUser)
            .LoadAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, ToReviewResponseDto(review));
    }

    // PUT: api/Review/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, ReviewDto reviewDto)
    {
        var userId = User.GetObjectId();
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == userId);
        
        if (currentUser == null)
            return NotFound("Current user not found");

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        // Check if the current user is either the reviewer or the user being reviewed
        if (review.FromUserId != currentUser.Id && review.ForUserId != currentUser.Id)
            return Forbid();

        review.Q1 = reviewDto.Q1;
        review.A1 = reviewDto.A1;
        review.Q2 = reviewDto.Q2;
        review.A2 = reviewDto.A2;
        review.Q3 = reviewDto.Q3;
        review.A3 = reviewDto.A3;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReviewExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // GET: api/Review/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetReview(int id)
    {
        var userId = User.GetObjectId();
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == userId);
        
        if (currentUser == null)
            return NotFound("Current user not found");

        var review = await _context.Reviews
            .Include(r => r.ForUser)
            .Include(r => r.FromUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
            return NotFound();

        // Check if the current user is either the reviewer or the user being reviewed
        if (review.FromUserId != currentUser.Id && review.ForUserId != currentUser.Id)
            return Forbid();

        return ToReviewResponseDto(review);
    }

    // GET: api/Review
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviews([FromQuery] int? quarter, [FromQuery] int? year, [FromQuery] int? userId)
    {
        var currentUserId = User.GetObjectId();
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == currentUserId);
        
        if (currentUser == null)
            return NotFound("Current user not found");

        var query = _context.Reviews
            .Include(r => r.ForUser)
            .Include(r => r.FromUser)
            .Where(r => r.FromUserId == currentUser.Id || r.ForUserId == currentUser.Id);

        if (quarter.HasValue)
            query = query.Where(r => r.Quarter == quarter.Value);
        
        if (year.HasValue)
            query = query.Where(r => r.Year == year.Value);
        
        if (userId.HasValue)
            query = query.Where(r => r.ForUserId == userId.Value);

        var reviews = await query.ToListAsync();
        return reviews.Select(ToReviewResponseDto).ToList();
    }

    private bool ReviewExists(int id)
    {
        return _context.Reviews.Any(e => e.Id == id);
    }

    // DELETE: api/Review/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = User.GetObjectId();
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == userId);
        
        if (currentUser == null)
            return NotFound("Current user not found");

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        // Only allow the reviewer (FromUser) to delete the review
        if (review.FromUserId != currentUser.Id)
            return Forbid();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ReviewResponseDto ToReviewResponseDto(Review review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            ForUserId = review.ForUserId,
            FromUserId = review.FromUserId,
            Year = review.Year,
            Quarter = review.Quarter,
            Q1 = review.Q1,
            A1 = review.A1,
            Q2 = review.Q2,
            A2 = review.A2,
            Q3 = review.Q3,
            A3 = review.A3,
            ForUser = new UserBasicInfoDto
            {
                Id = review.ForUser.Id,
                Name = review.ForUser.Name,
                Email = review.ForUser.Email
            },
            FromUser = new UserBasicInfoDto
            {
                Id = review.FromUser.Id,
                Name = review.FromUser.Name,
                Email = review.FromUser.Email
            }
        };
    }
}