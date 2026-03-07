using BetFounders.CentralBackend.Data.Entities.Roles;

namespace BetFounders.CentralBackend.Data.Entities.Users;

public class User
{
    public long Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public long RoleId { get; set; }
    
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Role? Role { get; set; }
}