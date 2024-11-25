using System.ComponentModel.DataAnnotations;

namespace pms_backend.Models;

public class ReviewDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public int Quarter { get; set; }

    [Required]
    public string Q1 { get; set; } = string.Empty;
    public string A1 { get; set; } = string.Empty;

    [Required]
    public string Q2 { get; set; } = string.Empty;
    public string A2 { get; set; } = string.Empty;

    [Required]
    public string Q3 { get; set; } = string.Empty;
    public string A3 { get; set; } = string.Empty;
}
