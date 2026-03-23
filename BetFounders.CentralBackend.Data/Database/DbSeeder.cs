using BetFounders.CentralBackend.Data.Constants;
using Dapper;
using System.Data;
namespace BetFounders.CentralBackend.Data.Database;

public static class DbSeeder
{
    public static async Task SeedAsync(DbConnectionFactory factory)
    {
        await SeedRolesAsync(factory);
        await SeedAdminUserAsync(factory);
    }

    private static async Task SeedRolesAsync(DbConnectionFactory factory)
    {
        var roles = new[]
        {
            new { p_Name = UserRoles.Admin,   p_Description = UserRoles.AdminDescription },
            new { p_Name = UserRoles.Manager, p_Description = UserRoles.ManagerDescription },
            new { p_Name = UserRoles.Viewer,  p_Description = UserRoles.ViewerDescription }
        };

        using var conn = factory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_SeedRole",
            roles,
            commandType: CommandType.StoredProcedure
        );
    }

    private static async Task SeedAdminUserAsync(DbConnectionFactory factory)
    {
        using var conn = factory.CreateConnection();

        var adminExists = await conn.ExecuteScalarAsync<bool>(
            "sp_UsernameExists",
            new { p_Username = "admin", p_ExcludeId = (long?)null },
            commandType: CommandType.StoredProcedure
        );

        if (adminExists)
        {
            return;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        await conn.ExecuteScalarAsync<long>(
            "sp_CreateUser",
            new
            {
                p_Username = "admin",
                p_Email = "super_admin@betfounders.com",
                p_PasswordHash = passwordHash,
                p_FirstName = "System",
                p_LastName = "Admin",
                p_RoleId = UserRoles.AdminId,
                p_IsActive = true
            },
            commandType: CommandType.StoredProcedure
        );
    }
}