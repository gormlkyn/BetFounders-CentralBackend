using BetFounders.CentralBackend.Data.Database;
using BetFounders.CentralBackend.Data.Entities.Roles;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Dapper;
using System.Data;

namespace BetFounders.CentralBackend.Data.Repositories;

public class RoleRepository(DbConnectionFactory dbFactory) : IRoleRepository
{
    public async Task<IEnumerable<Role>> GetRolesForSelectionAsync()
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.QueryAsync<Role>(
            "sp_GetRolesForSelection",
            commandType: CommandType.StoredProcedure
        );
    }
}