using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
    }
}
