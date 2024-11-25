using System.ComponentModel.DataAnnotations;

namespace pms_backend.Models;

public class AssignManagerRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ManagerId { get; set; }
}
