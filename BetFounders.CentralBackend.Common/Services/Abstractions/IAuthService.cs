using BetFounders.CentralBackend.Data.Entities.Users;

namespace BetFounders.CentralBackend.Common.Services.Abstractions;

public interface IAuthService
{
    Task<User> LoginAsync(string username, string password, string ipAddress, string userAgent);
}
