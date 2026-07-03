namespace WildlifeConservation.Api.DTOs.Species;

public class SpeciesResponseProfile : Profile
{
    public SpeciesResponseProfile()
    {
        CreateMap<Models.Species.Species, SpeciesResponseDto>();
    }
}
