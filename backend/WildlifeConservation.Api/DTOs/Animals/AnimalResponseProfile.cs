using WildlifeConservation.Models.Animals;

namespace WildlifeConservation.Api.DTOs.Animals;

public class AnimalResponseProfile : Profile
{
    public AnimalResponseProfile()
    {
        CreateMap<Animal, AnimalResponseDto>();
    }
}
