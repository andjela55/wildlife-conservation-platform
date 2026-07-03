using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Collars;

public class CollarProfile : Profile
{
    public CollarProfile()
    {
        CreateMap<CreateCollarDto, Collar>();
        CreateMap<UpdateCollarDto, Collar>();
    }
}
