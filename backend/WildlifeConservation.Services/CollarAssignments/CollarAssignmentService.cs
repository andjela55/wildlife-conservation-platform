namespace WildlifeConservation.Services.CollarAssignments;

public class CollarAssignmentService(
    ICollarAssignmentRepository collarAssignmentRepository,
    ICollarRepository collarRepository,
    ICollarAssignmentValidationService validationService,
    ITransactionService transactionService,
    IMapper mapper) : ICollarAssignmentService
{
    public async Task<PagedResult<CollarAssignment>> GetAllAsync(CollarAssignmentQuery query, CancellationToken cancellationToken = default)
    {
        validationService.ValidateQuery(query);

        var assignments = collarAssignmentRepository.Query();
        if (query.AnimalId.HasValue)
        {
            assignments = assignments.Where(x => x.AnimalId == query.AnimalId.Value);
        }
        if (query.AssignedFrom.HasValue)
        {
            var assignedFrom = ServiceHelpers.AsUtc(query.AssignedFrom.Value);
            assignments = assignments.Where(x => x.AssignedAt >= assignedFrom);
        }
        if (query.AssignedTo.HasValue)
        {
            var assignedTo = ServiceHelpers.AsUtc(query.AssignedTo.Value);
            assignments = assignments.Where(x => x.AssignedAt <= assignedTo);
        }
        if (query.ActiveOnly.HasValue)
        {
            assignments = query.ActiveOnly.Value
                ? assignments.Where(x => x.UnassignedAt == null)
                : assignments.Where(x => x.UnassignedAt != null);
        }

        return await assignments.OrderByDescending(x => x.AssignedAt).ToPagedResultAsync(query, cancellationToken);
    }

    public async Task<PagedResult<CollarAssignment>> GetActiveAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await collarAssignmentRepository.Query()
            .Where(x => x.UnassignedAt == null)
            .OrderBy(x => x.AnimalId)
            .ThenBy(x => x.CollarId)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<CollarAssignment> CreateAsync(CreateCollarAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var collar = await validationService.ValidateCreateAsync(dto, cancellationToken);

        var assignment = mapper.Map<CollarAssignment>(dto);
        collar.Status = CollarStatus.Assigned;

        return await transactionService.ExecuteAsync(async () =>
        {
            await collarRepository.UpdateAsync(collar, cancellationToken);
            return await collarAssignmentRepository.InsertAsync(assignment, cancellationToken);
        }, cancellationToken);
    }

    public async Task<CollarAssignment> UnassignAsync(int id, UnassignCollarDto dto, CancellationToken cancellationToken = default)
    {
        var (assignment, unassignedAt) = await validationService.ValidateUnassignAsync(id, dto, cancellationToken);

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
