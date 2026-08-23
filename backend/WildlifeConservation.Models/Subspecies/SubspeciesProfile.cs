using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Subspecies;

public class SubspeciesProfile : Profile
{
    public SubspeciesProfile()
    {
        CreateMap<UpsertSubspeciesDto, Subspecies>().AfterMap(Normalize);
    }

    private static void Normalize(UpsertSubspeciesDto source, Subspecies destination)
    {
        destination.Name = source.Name.Trim();
        destination.Description = source.Description.Trim();
    }
}
