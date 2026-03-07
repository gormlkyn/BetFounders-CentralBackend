namespace BetFounders.CentralBackend.Common.Models.Users;

public class UserGridModel
{
    public long Id { get; set; }

    public string FullName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string RoleName { get; set; }

    public string RoleDescription { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}