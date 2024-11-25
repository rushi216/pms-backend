using System.ComponentModel.DataAnnotations;

namespace pms_backend.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string MicrosoftId { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<UserManagerMapping> DirectReports { get; set; } = new List<UserManagerMapping>();
    public ICollection<UserManagerMapping> ManagerMappings { get; set; } = new List<UserManagerMapping>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
}
