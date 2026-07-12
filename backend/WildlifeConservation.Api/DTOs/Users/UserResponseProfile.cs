using AutoMapper;
using WildlifeConservation.Models.Users;

namespace WildlifeConservation.Api.DTOs.Users;

public class UserResponseProfile : Profile
{
    public UserResponseProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}
