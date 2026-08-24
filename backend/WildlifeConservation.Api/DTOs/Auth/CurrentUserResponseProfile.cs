using WildlifeConservation.Models.Users;

namespace WildlifeConservation.Api.DTOs.Auth;

public class CurrentUserResponseProfile : Profile
{
    public CurrentUserResponseProfile()
    {
        CreateMap<User, CurrentUserResponseDto>()
            .ForCtorParam(nameof(CurrentUserResponseDto.Roles), options => options.MapFrom(source =>
                source.UserRoles.Select(x => x.Role)))
            .ForCtorParam(nameof(CurrentUserResponseDto.Permissions), options => options.MapFrom(source =>
                source.UserRoles
                    .SelectMany(x => x.Role.RolePermissions)
                    .Select(x => x.Permission.Code)
                    .Distinct()));
    }
}
