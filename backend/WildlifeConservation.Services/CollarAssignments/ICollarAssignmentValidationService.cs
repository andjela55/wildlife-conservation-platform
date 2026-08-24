namespace WildlifeConservation.Services.CollarAssignments;

public interface ICollarAssignmentValidationService
{
    void ValidateQuery(CollarAssignmentQuery query);
    Task<Collar> ValidateCreateAsync(CreateCollarAssignmentDto dto, CancellationToken cancellationToken);
    Task<(CollarAssignment Assignment, DateTime UnassignedAt)> ValidateUnassignAsync(
        int id,
        UnassignCollarDto dto,
        CancellationToken cancellationToken);
}
