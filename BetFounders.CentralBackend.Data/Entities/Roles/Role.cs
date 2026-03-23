namespace BetFounders.CentralBackend.Data.Entities.Roles;

public class Role
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}