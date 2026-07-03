namespace WildlifeConservation.Services.Alerts;

public class AlertService(
    IAlertRepository alertRepository,
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository,
    IUserRepository userRepository,
    IMapper mapper) : IAlertService
{
    public async Task<List<Alert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var alerts = await alertRepository.Query()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return alerts;
    }

    public async Task<Alert> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return alert is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Alert with id {id} was not found.")
            : alert;
    }

    public async Task<Alert> CreateAsync(CreateAlertDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");

        if (dto.CollarId.HasValue)
        {
            await ServiceHelpers.EnsureFoundAsync(collarRepository.GetByIdAsync(dto.CollarId.Value, cancellationToken), dto.CollarId.Value, "Collar");
        }

        if (dto.CreatedByUserId.HasValue)
        {
            await ServiceHelpers.EnsureFoundAsync(userRepository.GetByIdAsync(dto.CreatedByUserId.Value, cancellationToken), dto.CreatedByUserId.Value, "User");
        }

        var alert = mapper.Map<Alert>(dto);
        alert.Description = ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        alert.IsResolved = false;
        alert.CreatedAt = ServiceHelpers.AsUtc(dto.CreatedAt);

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
