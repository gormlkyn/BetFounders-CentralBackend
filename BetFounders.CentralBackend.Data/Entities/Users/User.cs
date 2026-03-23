using BetFounders.CentralBackend.Data.Entities.Roles;

namespace BetFounders.CentralBackend.Data.Entities.Users;

public class User
{
    public long Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public long RoleId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Role? Role { get; set; }
}