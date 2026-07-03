using WildlifeConservation.Models.LocationPoints;

namespace WildlifeConservation.Api.DTOs.LocationPoints;

public class LocationPointResponseProfile : Profile
{
    public LocationPointResponseProfile()
    {
        CreateMap<LocationPoint, LocationPointResponseDto>();
    }
}
