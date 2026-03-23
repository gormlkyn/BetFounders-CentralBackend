using AutoMapper;
using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Common.Services.Abstractions;
using BetFounders.CentralBackend.Data.Repositories.Abstractions;

namespace BetFounders.CentralBackend.Common.Services;

public class RoleService(IRoleRepository roleRepo, IMapper mapper) : IRoleService
{
    public async Task<IEnumerable<EntityDropDownModel>> GetRoleDropDownAsync()
    {
        var roles = await roleRepo.GetRolesForSelectionAsync();
        return mapper.Map<IEnumerable<EntityDropDownModel>>(roles);
    }
}