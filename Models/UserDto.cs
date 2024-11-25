namespace pms_backend.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MicrosoftId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserManagerInfo? CurrentManager { get; set; }
}

public class UserManagerInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AssignmentTimestamp { get; set; }
}
