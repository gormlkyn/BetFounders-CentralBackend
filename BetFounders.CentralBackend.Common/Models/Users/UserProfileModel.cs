namespace BetFounders.CentralBackend.Common.Models.Users;

public class UserProfileModel
{
    public long Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string RoleName { get; set; }

    public string RoleDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
