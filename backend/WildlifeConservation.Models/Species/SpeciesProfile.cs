using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.Species;

public class SpeciesProfile : Profile
{
    public SpeciesProfile()
    {
        CreateMap<UpsertSpeciesDto, Species>().AfterMap(Normalize);
    }

    private static void Normalize(UpsertSpeciesDto source, Species destination)
    {
        destination.Name = source.Name.Trim();
        destination.Description = source.Description.Trim();
    }
}
