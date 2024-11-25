using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pms_backend.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ForUserId { get; set; }

    [Required]
    public int FromUserId { get; set; }

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

    // Navigation properties
    [ForeignKey("ForUserId")]
    public User ForUser { get; set; } = null!;

    [ForeignKey("FromUserId")]
    public User FromUser { get; set; } = null!;
}
