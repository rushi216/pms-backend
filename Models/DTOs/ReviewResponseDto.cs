using System.ComponentModel.DataAnnotations;

namespace pms_backend.Models;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public int ForUserId { get; set; }
    public int FromUserId { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    public string Q1 { get; set; } = string.Empty;
    public string A1 { get; set; } = string.Empty;
    public string Q2 { get; set; } = string.Empty;
    public string A2 { get; set; } = string.Empty;
    public string Q3 { get; set; } = string.Empty;
    public string A3 { get; set; } = string.Empty;
    
    // Include only necessary user information
    public UserBasicInfoDto ForUser { get; set; } = null!;
    public UserBasicInfoDto FromUser { get; set; } = null!;
}
