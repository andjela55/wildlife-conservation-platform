using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.LocationPoints;

public class LocationPointProfile : Profile
{
    public LocationPointProfile()
    {
        CreateMap<CreateLocationPointDto, LocationPoint>().AfterMap((source, destination) =>
        {
            destination.RecordedAt = InputNormalization.AsUtc(source.RecordedAt);
            destination.Notes = InputNormalization.TrimOptional(source.Notes);
        });
    }
}
