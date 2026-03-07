using BetFounders.CentralBackend.Common.Models;

namespace BetFounders.CentralBackend.Common.Services.Abstractions;

public interface IRoleService
{
    Task<IEnumerable<EntityDropDownModel>> GetRoleDropDown();
}