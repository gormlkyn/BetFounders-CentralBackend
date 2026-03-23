using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Common.Services.Abstractions;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Microsoft.Extensions.Logging;

namespace BetFounders.CentralBackend.Common.Services;

public class AuthService(IUserRepository userRepo, ILogger<AuthService> logger) : IAuthService
{
    public async Task<ServiceResult<User>> LoginAsync(string username, string password, string ipAddress, string userAgent)
    {
        try
        {
            var user = await userRepo.GetByUsernameAsync(username);

            bool passwordCorrect = user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            bool success = passwordCorrect && user!.IsActive;

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
                    return ServiceResult<User>.Failure("Your account is inactive. Please contact your administrator.");
                }
            }

            if (!success)
            {
                return ServiceResult<User>.Failure("Invalid username or password.");
            }

            return ServiceResult<User>.Success(user!);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login error for user: {Username}", username);
            return ServiceResult<User>.Failure("An unexpected error occurred during login.");
        }
    }
}