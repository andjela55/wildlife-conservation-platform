using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.LocationPoints;

public class LocationPointProfile : Profile
{
    public LocationPointProfile()
    {
        CreateMap<CreateLocationPointDto, LocationPoint>();
    }
}
