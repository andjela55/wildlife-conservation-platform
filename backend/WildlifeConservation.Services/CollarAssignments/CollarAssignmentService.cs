namespace WildlifeConservation.Services.CollarAssignments;

public class CollarAssignmentService(
    ICollarAssignmentRepository collarAssignmentRepository,
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository,
    ITransactionService transactionService,
    IMapper mapper) : ICollarAssignmentService
{
    public async Task<List<CollarAssignment>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await collarAssignmentRepository.Query()
            .Where(x => x.UnassignedAt == null)
            .OrderBy(x => x.AnimalId)
            .ThenBy(x => x.CollarId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CollarAssignment> CreateAsync(CreateCollarAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");

        var collar = await collarRepository.GetByIdAsync(dto.CollarId, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar with id {dto.CollarId} was not found.");

        if (collar.Status is CollarStatus.Inactive or CollarStatus.Lost or CollarStatus.Damaged)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Only available or assignable collars can be assigned.");
        }

        var animalHasActiveAssignment = await collarAssignmentRepository.Query()
            .AnyAsync(x => x.AnimalId == dto.AnimalId && x.UnassignedAt == null, cancellationToken);

        if (animalHasActiveAssignment)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Animal already has an active collar assignment.");
        }

        var collarHasActiveAssignment = await collarAssignmentRepository.Query()
            .AnyAsync(x => x.CollarId == dto.CollarId && x.UnassignedAt == null, cancellationToken);

        if (collarHasActiveAssignment)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Collar already has an active assignment.");
        }

        var assignment = mapper.Map<CollarAssignment>(dto);
        assignment.AssignedAt = ServiceHelpers.AsUtc(dto.AssignedAt);
        assignment.Reason = dto.Reason?.Trim();
        assignment.Notes = dto.Notes?.Trim();
        collar.Status = CollarStatus.Assigned;

        return await transactionService.ExecuteAsync(async () =>
        {
            await collarRepository.UpdateAsync(collar, cancellationToken);
            return await collarAssignmentRepository.InsertAsync(assignment, cancellationToken);
        }, cancellationToken);
    }

    public async Task<CollarAssignment> UnassignAsync(int id, UnassignCollarDto dto, CancellationToken cancellationToken = default)
    {
        var assignment = await collarAssignmentRepository.GetByIdAsync(id, cancellationToken)
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

        assignment.UnassignedAt = unassignedAt;
        assignment.Reason = dto.Reason?.Trim() ?? assignment.Reason;
        assignment.Notes = dto.Notes?.Trim() ?? assignment.Notes;

        var collar = await collarRepository.GetByIdAsync(assignment.CollarId, cancellationToken);
        if (collar is not null && collar.Status == CollarStatus.Assigned)
        {
            collar.Status = CollarStatus.Available;
        }

        return await transactionService.ExecuteAsync(async () =>
        {
            var updatedAssignment = await collarAssignmentRepository.UpdateAsync(assignment, cancellationToken);

            if (collar is not null)
            {
                await collarRepository.UpdateAsync(collar, cancellationToken);
            }

            return updatedAssignment;
        }, cancellationToken);
    }
}
