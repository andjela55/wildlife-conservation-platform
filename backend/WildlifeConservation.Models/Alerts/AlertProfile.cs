using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.Alerts;

public class AlertProfile : Profile
{
    public AlertProfile()
    {
        CreateMap<CreateAlertDto, Alert>()
            .AfterMap((source, destination) => destination.Description = source.Description.Trim());
    }
}
