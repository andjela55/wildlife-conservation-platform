using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Species;

public class SpeciesProfile : Profile
{
    public SpeciesProfile()
    {
        CreateMap<CreateSpeciesDto, Species>();
    }
}
