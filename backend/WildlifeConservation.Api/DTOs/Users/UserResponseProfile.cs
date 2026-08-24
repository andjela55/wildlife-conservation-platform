using AutoMapper;
using WildlifeConservation.Models.Users;

namespace WildlifeConservation.Api.DTOs.Users;

public class UserResponseProfile : Profile
{
    public UserResponseProfile()
    {
        CreateMap<Role, RoleResponseDto>()
            .ForCtorParam(nameof(RoleResponseDto.Permissions), options => options.MapFrom(source =>
                source.RolePermissions.Select(x => x.Permission.Code)));
        CreateMap<User, UserResponseDto>()
            .ForCtorParam(nameof(UserResponseDto.Roles), options => options.MapFrom(source =>
                source.UserRoles.Select(x => x.Role)));
    }
}
