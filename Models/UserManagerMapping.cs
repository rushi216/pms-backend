using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pms_backend.Models;

public class UserManagerMapping
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ManagerId { get; set; }

    [Required]
    public DateTime AssignmentTimestamp { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("ManagerId")]
    public User Manager { get; set; } = null!;
}
