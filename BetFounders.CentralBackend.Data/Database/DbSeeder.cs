using Dapper;

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
            new { Name = "Admin",   Description = "Full access" },
            new { Name = "Manager", Description = "Can manage users" },
            new { Name = "Viewer",  Description = "Read only access" }
        };

        using var conn = factory.CreateConnection();
        var sql = @"
            INSERT INTO Roles (Name, Description)
            SELECT @Name, @Description
            WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Name = @Name)";

        await conn.ExecuteAsync(sql, roles);
    }

    private static async Task SeedAdminUserAsync(DbConnectionFactory factory)
    {
        using var conn = factory.CreateConnection();

        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users WHERE Username = 'admin'");
        if (count > 0) return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        var sql = @"
            INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, RoleId, IsActive, CreatedAt)
            SELECT @Username, @Email, @PasswordHash, @FirstName, @LastName, r.Id, 1, NOW()
            FROM Roles r
            WHERE r.Name = 'Admin'";

        await conn.ExecuteAsync(sql, new
        {
            Username = "admin",
            Email = "super_admin@betfounders.com",
            PasswordHash = passwordHash,
            FirstName = "System",
            LastName = "Admin"
        });
    }
}