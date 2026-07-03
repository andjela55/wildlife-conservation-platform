using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.Alerts;

public class AlertProfile : Profile
{
    public AlertProfile()
    {
        CreateMap<CreateAlertDto, Alert>();
    }
}
