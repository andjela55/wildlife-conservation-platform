namespace WildlifeConservation.Services.CollarAssignments;

public class CollarAssignmentValidationService(
    ICollarAssignmentRepository assignmentRepository,
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository) : ICollarAssignmentValidationService
{
    public void ValidateQuery(CollarAssignmentQuery query)
    {
        if (query.AssignedFrom.HasValue && query.AssignedTo.HasValue && query.AssignedFrom > query.AssignedTo)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "AssignedFrom cannot be later than AssignedTo.");
        }
    }

    public async Task<Collar> ValidateCreateAsync(CreateCollarAssignmentDto dto, CancellationToken cancellationToken)
    {
        await ServiceHelpers.EnsureFoundAsync(
            animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");
        var collar = await collarRepository.GetByIdAsync(dto.CollarId, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar with id {dto.CollarId} was not found.");
        if (collar.Status is CollarStatus.Inactive or CollarStatus.Lost or CollarStatus.Damaged)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Only available or assignable collars can be assigned.");
        }

        if (await assignmentRepository.Query().AnyAsync(x => x.AnimalId == dto.AnimalId && x.UnassignedAt == null, cancellationToken))
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Animal already has an active collar assignment.");
        }
        if (await assignmentRepository.Query().AnyAsync(x => x.CollarId == dto.CollarId && x.UnassignedAt == null, cancellationToken))
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Collar already has an active assignment.");
        }

        return collar;
    }

    public async Task<(CollarAssignment Assignment, DateTime UnassignedAt)> ValidateUnassignAsync(
        int id,
        UnassignCollarDto dto,
        CancellationToken cancellationToken)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar assignment with id {id} was not found.");
        if (assignment.UnassignedAt.HasValue)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Collar assignment is already unassigned.");
        }

        var unassignedAt = ServiceHelpers.AsUtc(dto.UnassignedAt ?? DateTime.UtcNow);
        if (unassignedAt < assignment.AssignedAt)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "UnassignedAt cannot be earlier than AssignedAt.");
        }

        return (assignment, unassignedAt);
    }
}
