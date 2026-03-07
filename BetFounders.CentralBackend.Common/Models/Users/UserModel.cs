namespace BetFounders.CentralBackend.Common.Models.Users;

public class UserModel
{
    public long Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public long? RoleId { get; set; }
}