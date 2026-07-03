using WildlifeConservation.Models.Alerts;

namespace WildlifeConservation.Api.DTOs.Alerts;

public class AlertResponseProfile : Profile
{
    public AlertResponseProfile()
    {
        CreateMap<Alert, AlertResponseDto>();
    }
}
