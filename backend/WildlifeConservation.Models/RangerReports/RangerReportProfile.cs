using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.RangerReports;

public class RangerReportProfile : Profile
{
    public RangerReportProfile()
    {
        CreateMap<CreateRangerReportDto, RangerReport>();
    }
}
