namespace WildlifeConservation.Services.CollarAssignments;

public interface ICollarAssignmentService
{
    Task<CollarAssignment> CreateAsync(CreateCollarAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<CollarAssignment> UnassignAsync(int id, UnassignCollarDto dto, CancellationToken cancellationToken = default);
}
