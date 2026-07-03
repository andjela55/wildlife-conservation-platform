using WildlifeConservation.Models.CollarAssignments;

namespace WildlifeConservation.Api.DTOs.CollarAssignments;

public class CollarAssignmentResponseProfile : Profile
{
    public CollarAssignmentResponseProfile()
    {
        CreateMap<CollarAssignment, CollarAssignmentResponseDto>();
    }
}
