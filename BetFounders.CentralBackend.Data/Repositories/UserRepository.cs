using BetFounders.CentralBackend.Data.Database;
using BetFounders.CentralBackend.Data.Entities.Roles;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Dapper;
using System.Data;

namespace BetFounders.CentralBackend.Data.Repositories;

public class UserRepository(DbConnectionFactory dbFactory) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.QueryAsync<User, Role, User>(
            "sp_GetAllUsers",
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { p_ExcludeId = excludeId },
            splitOn: "Id",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        using var conn = dbFactory.CreateConnection();

        var users = await conn.QueryAsync<User, Role, User>(
            "sp_GetUserById",
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { p_Id = id },
            splitOn: "Id",
            commandType: CommandType.StoredProcedure
        );

        return users.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = dbFactory.CreateConnection();

        var users = await conn.QueryAsync<User, Role, User>(
            "sp_GetUserByUsername",
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { p_Username = username },
            splitOn: "Id",
            commandType: CommandType.StoredProcedure
        );

        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<UserLoginHistory>> GetLoginsByUserIdAsync(long userId)
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.QueryAsync<UserLoginHistory>(
            "sp_GetLoginsByUserId",
            new { p_UserId = userId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<bool> EmailExistsAsync(string email, long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.ExecuteScalarAsync<bool>(
            "sp_EmailExists",
            new { p_Email = email, p_ExcludeId = excludeId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<bool> UsernameExistsAsync(string username, long? excludeId = null)
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.ExecuteScalarAsync<bool>(
            "sp_UsernameExists",
            new { p_Username = username, p_ExcludeId = excludeId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task AddLoginAsync(UserLoginHistory history)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_AddLoginHistory",
            new
            {
                p_UserId = history.UserId,
                p_IpAddress = history.IpAddress,
                p_UserAgent = history.UserAgent,
                p_IsSuccess = history.IsSuccess
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<long> CreateAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        return await conn.ExecuteScalarAsync<long>(
            "sp_CreateUser",
            new
            {
                p_Username = user.Username,
                p_Email = user.Email,
                p_PasswordHash = user.PasswordHash,
                p_FirstName = user.FirstName,
                p_LastName = user.LastName,
                p_RoleId = user.RoleId,
                p_IsActive = user.IsActive
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_UpdateUser",
            new
            {
                p_Id = user.Id,
                p_Email = user.Email,
                p_Username = user.Username,
                p_FirstName = user.FirstName,
                p_LastName = user.LastName,
                p_RoleId = user.RoleId
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task UpdateProfileAsync(User user)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_UpdateUserProfile",
            new
            {
                p_Id = user.Id,
                p_Email = user.Email,
                p_Username = user.Username,
                p_FirstName = user.FirstName,
                p_LastName = user.LastName
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task UpdateStatusAsync(long id, bool newStatus)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_UpdateUserStatus",
            new
            {
                p_IsActive = newStatus,
                p_Id = id
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task UpdatePasswordAsync(long userId, string passwordHash)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_UpdateUserPassword",
            new
            {
                p_UserId = userId,
                p_PasswordHash = passwordHash
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task DeleteAsync(long id)
    {
        using var conn = dbFactory.CreateConnection();

        await conn.ExecuteAsync(
            "sp_DeleteUser",
            new { p_Id = id },
            commandType: CommandType.StoredProcedure
        );
    }
}