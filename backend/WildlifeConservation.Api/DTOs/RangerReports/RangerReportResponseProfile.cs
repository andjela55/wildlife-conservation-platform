using WildlifeConservation.Models.RangerReports;

namespace WildlifeConservation.Api.DTOs.RangerReports;

public class RangerReportResponseProfile : Profile
{
    public RangerReportResponseProfile()
    {
        CreateMap<RangerReport, RangerReportResponseDto>();
    }
}
