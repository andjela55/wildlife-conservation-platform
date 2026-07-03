using AutoMapper;
using WildlifeConservation.DTOs;

namespace WildlifeConservation.Models.CollarAssignments;

public class CollarAssignmentProfile : Profile
{
    public CollarAssignmentProfile()
    {
        CreateMap<CreateCollarAssignmentDto, CollarAssignment>();
    }
}
