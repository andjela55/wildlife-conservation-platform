using AutoMapper;
using WildlifeConservation.DTOs;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Models.CollarAssignments;

public class CollarAssignmentProfile : Profile
{
    public CollarAssignmentProfile()
    {
        CreateMap<CreateCollarAssignmentDto, CollarAssignment>().AfterMap((source, destination) =>
        {
            destination.AssignedAt = InputNormalization.AsUtc(source.AssignedAt);
            destination.Reason = InputNormalization.TrimOptional(source.Reason);
            destination.Notes = InputNormalization.TrimOptional(source.Notes);
        });
    }
}
