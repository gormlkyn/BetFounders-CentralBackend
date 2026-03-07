namespace BetFounders.CentralBackend.Data.Entities.Roles;

public class Role
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
}