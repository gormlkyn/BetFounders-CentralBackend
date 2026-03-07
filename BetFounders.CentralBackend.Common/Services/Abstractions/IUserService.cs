using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Common.Models.Users;

namespace BetFounders.CentralBackend.Common.Services.Abstractions;

public interface IUserService
{
    Task<IEnumerable<UserGridModel>> GetAllAsync(long excludeId);

    Task<IEnumerable<UserDashboardModel>> GetAllForDashboardAsync();

    Task<UserModel> GetByIdAsync(long id);

    Task<UserProfileModel> GetProfileModelAsync(long id);

    Task<IEnumerable<LoginDashboardModel>> GetUserLoginsAsync(long userId);

    Task<ServiceResult<long>> CreateAsync(UserModel model);

    Task<ServiceResult<bool>> UpdateAsync(UserModel model);

    Task<ServiceResult<bool>> UpdateAsync(UserProfileModel model);

    Task<ServiceResult<bool>> UpdateStatusAsync(long id, bool newStatus);

    Task<ServiceResult<bool>> ChangePasswordAsync(UserPasswordModel model);

    Task<ServiceResult<bool>> DeleteAsync(long id);
}