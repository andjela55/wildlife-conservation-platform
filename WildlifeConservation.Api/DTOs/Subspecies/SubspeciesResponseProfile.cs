namespace WildlifeConservation.Api.DTOs.Subspecies;

public class SubspeciesResponseProfile : Profile
{
    public SubspeciesResponseProfile()
    {
        CreateMap<Models.Subspecies.Subspecies, SubspeciesResponseDto>();
    }
}
