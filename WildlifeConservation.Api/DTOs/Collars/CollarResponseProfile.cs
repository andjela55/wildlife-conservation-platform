using WildlifeConservation.Models.Collars;

namespace WildlifeConservation.Api.DTOs.Collars;

public class CollarResponseProfile : Profile
{
    public CollarResponseProfile()
    {
        CreateMap<Collar, CollarResponseDto>();
    }
}
