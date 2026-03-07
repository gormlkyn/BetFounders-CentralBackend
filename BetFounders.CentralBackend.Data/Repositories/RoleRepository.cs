using BetFounders.CentralBackend.Data.Database;
using BetFounders.CentralBackend.Data.Entities.Roles;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Dapper;

namespace BetFounders.CentralBackend.Data.Repositories;

public class RoleRepository(DbConnectionFactory dbFactory) : IRoleRepository
{
    public async Task<IEnumerable<Role>> GetRolesForSelectionAsync()
    {
        using var conn = dbFactory.CreateConnection();
        var sql = "SELECT Id, Name FROM Roles ORDER BY Name";

        return await conn.QueryAsync<Role>(sql);
    }
}