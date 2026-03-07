using BetFounders.CentralBackend.Data.Database;
using BetFounders.CentralBackend.Data.Entities.Roles;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Dapper;

namespace BetFounders.CentralBackend.Data.Repositories;

public class UserRepository(DbConnectionFactory dbFactory) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = $@"
        SELECT u.*, r.* FROM Users u
        INNER JOIN Roles r ON u.RoleId = r.Id
        {(excludeId.HasValue ? "WHERE u.Id != @ExcludeId" : "")}
        ORDER BY u.CreatedAt DESC";

        var users = await conn.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { ExcludeId = excludeId },
            splitOn: "Id"
        );

        return users;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        SELECT u.*, r.* FROM Users u
        INNER JOIN Roles r ON u.RoleId = r.Id
        WHERE u.Id = @Id";

        var users = await conn.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return users.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        SELECT u.*, r.* FROM Users u
        INNER JOIN Roles r ON u.RoleId = r.Id
        WHERE u.Username = @Username OR u.Email = @Username";

        var users = await conn.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Username = username }, // Binds to both @Username parameters in the SQL!
            splitOn: "Id"
        );

        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<UserLoginHistory>> GetLoginsByUserIdAsync(long userId)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        SELECT h.*, u.Username
        FROM UserLoginHistory h
        INNER JOIN Users u ON u.Id = h.UserId
        WHERE h.UserId = @UserId
        ORDER BY h.LoginAt DESC";

        return await conn.QueryAsync<UserLoginHistory>(sql, new { UserId = userId });
    }

    public async Task<bool> EmailExistsAsync(string email, long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = $@"
        SELECT EXISTS (
            SELECT 1 FROM Users 
            WHERE Email = @Email 
            {(excludeId.HasValue ? "AND Id != @ExcludeId" : "")})";

        return await conn.ExecuteScalarAsync<bool>(sql, new { Email = email, ExcludeId = excludeId });
    }

    public async Task<bool> UsernameExistsAsync(string username, long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = $@"
        SELECT EXISTS (
            SELECT 1 FROM Users 
            WHERE Username = @Username 
            {(excludeId.HasValue ? "AND Id != @ExcludeId" : "")})";

        return await conn.ExecuteScalarAsync<bool>(sql, new { Username = username, ExcludeId = excludeId });
    }

    public async Task AddLoginAsync(UserLoginHistory history)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        INSERT INTO UserLoginHistory (UserId, LoginAt, IpAddress, UserAgent, IsSuccess)
        VALUES (@UserId, NOW(), @IpAddress, @UserAgent, @IsSuccess)";

        await conn.ExecuteAsync(sql, history);
    }

    public async Task<long> CreateAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        INSERT INTO Users (
            Username, Email, PasswordHash, FirstName, LastName, 
            RoleId, IsActive, CreatedAt
        ) 
        VALUES (
            @Username, @Email, @PasswordHash, @FirstName, @LastName, 
            @RoleId, @IsActive, NOW()
        );
        SELECT LAST_INSERT_ID();";

        var newId = await conn.ExecuteScalarAsync<long>(sql, user);

        return newId;
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        UPDATE Users SET
        Email = @Email,
        Username = @Username,
        FirstName = @FirstName,
        LastName = @LastName,
        RoleId = @RoleId,
        UpdatedAt = NOW()
        WHERE Id = @Id";

        await conn.ExecuteAsync(sql, user);
    }

    public async Task UpdateProfileAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        UPDATE Users SET
        Email = @Email,
        Username = @Username,
        FirstName = @FirstName,
        LastName = @LastName,
        UpdatedAt = NOW()
        WHERE Id = @Id";

        await conn.ExecuteAsync(sql, user);
    }

    public async Task UpdateStatusAsync(long id, bool newStatus)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        UPDATE Users SET
        IsActive = @IsActive,
        UpdatedAt = NOW()
        WHERE Id = @Id";

        await conn.ExecuteAsync(sql, new { IsActive = newStatus, Id = id });
    }

    public async Task UpdatePasswordAsync(long userId, string passwordHash)
    {
        using var conn = dbFactory.CreateConnection();

        var sql = @"
        UPDATE Users SET 
        PasswordHash = @PasswordHash,
        UpdatedAt = NOW()
        WHERE Id = @UserId";

        await conn.ExecuteAsync(sql, new { UserId = userId, PasswordHash = passwordHash });
    }

    public async Task DeleteAsync(long id)
    {
        using var conn = dbFactory.CreateConnection();
        var sql = "DELETE FROM Users WHERE Id = @Id";

        await conn.ExecuteAsync(sql, new { Id = id });
    }
}