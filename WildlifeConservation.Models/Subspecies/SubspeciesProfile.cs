using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Subspecies;

public class SubspeciesProfile : Profile
{
    public SubspeciesProfile()
    {
        CreateMap<CreateSubspeciesDto, Subspecies>();
    }
}
