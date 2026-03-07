using BetFounders.CentralBackend.Data.Entities.Users;

namespace BetFounders.CentralBackend.Data.Repositories.Abstractions;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(long? Id = null);
    
    Task<User?> GetByIdAsync(long id);
    
    Task<User?> GetByUsernameAsync(string username);
    
    Task<IEnumerable<UserLoginHistory>> GetLoginsByUserIdAsync(long userId);

    Task<bool> EmailExistsAsync(string email, long? excludeId = null);

    Task<bool> UsernameExistsAsync(string username, long? excludeId = null);

    Task AddLoginAsync(UserLoginHistory history);

    Task<long> CreateAsync(User user);

    Task UpdateAsync(User user);

    Task UpdateProfileAsync(User user);

    Task UpdateStatusAsync(long id, bool newStatus);

    Task UpdatePasswordAsync(long userId, string passwordHash);

    Task DeleteAsync(long id);
}