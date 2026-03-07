using AutoMapper;
using BetFounders.CentralBackend.Common.Models;
using BetFounders.CentralBackend.Data.Entities.Roles;

namespace BetFounders.CentralBackend.Common.MappingProfiles.Roles;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, EntityDropDownModel>();
    }
}
