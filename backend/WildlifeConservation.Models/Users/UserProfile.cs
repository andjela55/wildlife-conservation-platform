using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>()
            .ForMember(x => x.UserRoles, options => options.Ignore())
            .AfterMap(Normalize);
        CreateMap<UpdateUserDto, User>()
            .ForMember(x => x.PasswordHash, options => options.Ignore())
            .ForMember(x => x.PasswordSalt, options => options.Ignore())
            .ForMember(x => x.UserRoles, options => options.Ignore())
            .AfterMap(Normalize);
        CreateMap<UpdateUserAssignedAreaDto, User>()
            .AfterMap((source, destination) => destination.AssignedLocationName = InputNormalization.TrimOptional(source.AssignedLocationName));
    }

    private static void Normalize(CreateUserDto source, User destination) => NormalizeValues(
        destination, source.FullName, source.Email, source.AssignedLocationName);

    private static void Normalize(UpdateUserDto source, User destination) => NormalizeValues(
        destination, source.FullName, source.Email, source.AssignedLocationName);

    private static void NormalizeValues(User destination, string fullName, string email, string? locationName)
    {
        destination.FullName = fullName.Trim();
        destination.Email = email.Trim().ToLowerInvariant();
        destination.AssignedLocationName = InputNormalization.TrimOptional(locationName);
    }
}
