using AutoMapper;
using BetFounders.CentralBackend.Common.Models.Users;
using BetFounders.CentralBackend.Data.Entities.Users;

namespace BetFounders.CentralBackend.Common.MappingProfiles.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserGridModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Unknown"))
            .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role != null ? src.Role.Description : "Unknown"));

        CreateMap<User, UserDashboardModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Unknown"));

        CreateMap<UserProfileModel, User>();
        CreateMap<User, UserProfileModel>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Unknown"))
            .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role != null ? src.Role.Description : "Unknown"));

        CreateMap<UserModel, User>();
        CreateMap<User, UserModel>();

        CreateMap<UserLoginHistory, LoginDashboardModel>();
    }
}