using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Animals;

public class AnimalProfile : Profile
{
    public AnimalProfile()
    {
        CreateMap<CreateAnimalDto, Animal>();
        CreateMap<UpdateAnimalDto, Animal>();
    }
}
