using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Data.Entities.Users;

namespace BetFounders.CentralBackend.Common.Services.Abstractions;

public interface IAuthService
{
    Task<ServiceResult<User>> LoginAsync(string username, string password, string ipAddress, string userAgent);
}
