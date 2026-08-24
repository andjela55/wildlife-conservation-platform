using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.Animals;

public class AnimalProfile : Profile
{
    public AnimalProfile()
    {
        CreateMap<UpsertAnimalDto, Animal>().AfterMap(Normalize);
    }

    private static void Normalize(UpsertAnimalDto source, Animal destination)
    {
        destination.Name = source.Name.Trim();
        destination.DateOfBirth = InputNormalization.AsUtc(source.DateOfBirth);
        destination.Notes = InputNormalization.TrimOptional(source.Notes);
    }
}
