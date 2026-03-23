using AutoMapper;
using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Common.Models.Users;
using BetFounders.CentralBackend.Common.Services.Abstractions;
using BetFounders.CentralBackend.Data.Constants;
using BetFounders.CentralBackend.Data.Entities.Users;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;
using Microsoft.Extensions.Logging;

namespace BetFounders.CentralBackend.Common.Services;

public class UserService(IUserRepository userRepo, IMapper mapper, ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserGridModel>> GetAllAsync(long excludeId)
    {
        var users = await userRepo.GetAllAsync(excludeId);
        return mapper.Map<IEnumerable<UserGridModel>>(users);
    }

    public async Task<IEnumerable<UserDashboardModel>> GetAllForDashboardAsync()
    {
        var users = await userRepo.GetAllAsync();
        return mapper.Map<IEnumerable<UserDashboardModel>>(users);
    }

    public async Task<UserModel> GetByIdAsync(long id)
    {
        var userEntity = await userRepo.GetByIdAsync(id);
        return mapper.Map<UserModel>(userEntity);
    }

    public async Task<UserProfileModel> GetProfileModelAsync(long id)
    {
        var userEntity = await userRepo.GetByIdAsync(id);
        return mapper.Map<UserProfileModel>(userEntity);
    }

    public async Task<IEnumerable<LoginDashboardModel>> GetUserLoginsAsync(long userId)
    {
        var logins = await userRepo.GetLoginsByUserIdAsync(userId);
        return mapper.Map<IEnumerable<LoginDashboardModel>>(logins);
    }

    public async Task<ServiceResult<long>> CreateAsync(UserModel model)
    {
        try
        {
            var normalizedEmail = model.Email?.Trim().ToLower();
            var normalizedUsername = model.Username?.Trim();

            var emailExists = await userRepo.EmailExistsAsync(normalizedEmail);
            if (emailExists)
            {
                return ServiceResult<long>.Failure("The email is already in use.");
            }

            var usernameExists = await userRepo.UsernameExistsAsync(normalizedUsername);
            if (usernameExists)
            {
                return ServiceResult<long>.Failure("The username is already in use.");
            }

            var userEntity = mapper.Map<User>(model);

            userEntity.Email = normalizedEmail;
            userEntity.Username = normalizedUsername;
            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newId = await userRepo.CreateAsync(userEntity);

            return ServiceResult<long>.Success(newId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create user with email: {Email}", model.Email);
            return ServiceResult<long>.Failure("An unexpected error occurred while creating the user.");
        }
    }
    public async Task<ServiceResult<long>> CreateViewerAsync(UserModel model)
    {
        model.RoleId = UserRoles.ViewerId;
        return await CreateAsync(model);
    }

    public Task<ServiceResult<bool>> UpdateAsync(UserModel model)
    {
        var userEntity = mapper.Map<User>(model);
        return ExecuteUpdateAsync(userEntity, isProfileUpdate: false);
    }

    public Task<ServiceResult<bool>> UpdateAsync(UserProfileModel profileModel)
    {
        var userEntity = mapper.Map<User>(profileModel);
        return ExecuteUpdateAsync(userEntity, isProfileUpdate: true);
    }

    private async Task<ServiceResult<bool>> ExecuteUpdateAsync(User userEntity, bool isProfileUpdate)
    {
        try
        {
            var normalizedEmail = userEntity.Email?.ToLower().Trim();
            var normalizedUsername = userEntity.Username?.Trim();

            var emailExists = await userRepo.EmailExistsAsync(normalizedEmail, userEntity.Id);
            if (emailExists)
            {
                return ServiceResult<bool>.Failure("The email is already in use.");
            }

            var usernameExists = await userRepo.UsernameExistsAsync(normalizedUsername, userEntity.Id);
            if (usernameExists)
            {
                return ServiceResult<bool>.Failure("The username is already in use.");
            }

            userEntity.Email = normalizedEmail;
            userEntity.Username = normalizedUsername;

            if (isProfileUpdate)
            {
                await userRepo.UpdateProfileAsync(userEntity);
            }
            else
            {
                await userRepo.UpdateAsync(userEntity);
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update user with ID: {UserId}. Profile update: {IsProfileUpdate}", userEntity.Id, isProfileUpdate);
            return ServiceResult<bool>.Failure("An error occurred while updating the user.");
        }
    }

    public async Task<ServiceResult<bool>> UpdateStatusAsync(long id, long currentUserId, bool newStatus)
    {
        if (id == currentUserId)
        {
            return ServiceResult<bool>.Failure("You cannot change your own status");
        }
        try
        {
            await userRepo.UpdateStatusAsync(id, newStatus);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update status for user with ID: {UserId} to {NewStatus}", id, newStatus);
            return ServiceResult<bool>.Failure("An error occurred while updating the user.");
        }
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(UserPasswordModel model)
    {
        try
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return ServiceResult<bool>.Failure("The new password and confirmation password do not match.");
            }

            var user = await userRepo.GetByIdAsync(model.UserId);
            if (user == null)
            {
                return ServiceResult<bool>.Failure("User not found.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                return ServiceResult<bool>.Failure("The current password you entered is incorrect.");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await userRepo.UpdatePasswordAsync(model.UserId, newPasswordHash);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to change password for user with ID: {UserId}", model.UserId);
            return ServiceResult<bool>.Failure("An error occurred while changing the password.");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(long id, long currentUserId)
    {
        if (id == currentUserId)
        {
            return ServiceResult<bool>.Failure("You cannot delete your own account.");
        }

        try
        {
            await userRepo.DeleteAsync(id);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user with ID: {UserId}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the user.");
        }
    }
}