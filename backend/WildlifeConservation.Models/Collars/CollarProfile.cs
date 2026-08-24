using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.Collars;

public class CollarProfile : Profile
{
    public CollarProfile()
    {
        CreateMap<UpsertCollarDto, Collar>().AfterMap(Normalize);
    }

    private static void Normalize(UpsertCollarDto source, Collar destination)
    {
        destination.SerialNumber = source.SerialNumber.Trim();
        destination.Model = InputNormalization.TrimOptional(source.Model);
        destination.Manufacturer = InputNormalization.TrimOptional(source.Manufacturer);
        destination.Notes = InputNormalization.TrimOptional(source.Notes);
    }
}
