using BetFounders.CentralBackend.Data.Entities.Roles;

namespace BetFounders.CentralBackend.Data.Repositories.Abstractions;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetRolesForSelectionAsync();
}