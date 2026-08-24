using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.RangerReports;

public class RangerReportProfile : Profile
{
    public RangerReportProfile()
    {
        CreateMap<CreateRangerReportDto, RangerReport>()
            .AfterMap((source, destination) => destination.Description = source.Description.Trim());
    }
}
