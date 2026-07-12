namespace WildlifeConservation.Services.Alerts;

public class AlertService(
    IAlertRepository alertRepository,
    IAnimalRepository animalRepository,
    ICollarAssignmentRepository collarAssignmentRepository,
    IUserRepository userRepository,
    IMapper mapper) : IAlertService
{
    public async Task<PagedResult<Alert>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await alertRepository.Query()
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Alert> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return alert is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Alert with id {id} was not found.")
            : alert;
    }

    public async Task<Alert> CreateAsync(CreateAlertDto dto, int createdByUserId, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");
        await ServiceHelpers.EnsureFoundAsync(userRepository.GetByIdAsync(createdByUserId, cancellationToken), createdByUserId, "User");

        var createdAt = ServiceHelpers.AsUtc(dto.CreatedAt);
        var activeAssignment = await collarAssignmentRepository.Query()
            .Where(x =>
                x.AnimalId == dto.AnimalId &&
                x.AssignedAt <= createdAt &&
                (x.UnassignedAt == null || x.UnassignedAt >= createdAt))
            .OrderByDescending(x => x.AssignedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var alert = mapper.Map<Alert>(dto);
        alert.CollarId = activeAssignment?.CollarId;
        alert.CreatedByUserId = createdByUserId;
        alert.Description = ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        alert.IsResolved = false;
        alert.CreatedAt = createdAt;

        alert = await alertRepository.InsertAsync(alert, cancellationToken);

        return alert;
    }

    public async Task<Alert> ResolveAsync(int id, ResolveAlertDto dto, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Alert with id {id} was not found.");

        if (alert.IsResolved)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Alert is already resolved.");
        }

        var resolvedAt = ServiceHelpers.AsUtc(dto.ResolvedAt ?? DateTime.UtcNow);
        if (resolvedAt < alert.CreatedAt)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "ResolvedAt cannot be earlier than CreatedAt.");
        }

        alert.IsResolved = true;
        alert.ResolvedAt = resolvedAt;

        alert = await alertRepository.UpdateAsync(alert, cancellationToken);

        return alert;
    }
}
