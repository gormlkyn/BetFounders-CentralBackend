using BetFounders.CentralBackend.Common.Services.Abstractions;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;

namespace BetFounders.CentralBackend.Common.Services;

public class AuthService(IUserRepository userRepo) : IAuthService
{
    public async Task<User> LoginAsync(string username, string password, string ipAddress, string userAgent)
    {
        var user = await userRepo.GetByUsernameAsync(username);

        bool passwordCorrect = user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        bool success = passwordCorrect && user.IsActive;

        if (user != null)
        {
            await userRepo.AddLoginAsync(new UserLoginHistory
            {
                UserId = user.Id,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                IsSuccess = success
            });

            if (passwordCorrect && !user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is inactive");
            }
        }

        return success ? user : null;
    }
}
