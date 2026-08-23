namespace WildlifeConservation.Services.Alerts;

public class AlertService(
    IAlertRepository alertRepository,
    IAlertValidationService validationService,
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
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Alert> CreateAsync(CreateAlertDto dto, int createdByUserId, CancellationToken cancellationToken = default)
    {
        var createdAt = DateTime.UtcNow;
        var collarId = await validationService.ValidateCreateAsync(dto, createdByUserId, createdAt, cancellationToken);

        var alert = mapper.Map<Alert>(dto);
        alert.CollarId = collarId;
        alert.CreatedByUserId = createdByUserId;
        alert.IsResolved = false;
        alert.CreatedAt = createdAt;

        alert = await alertRepository.InsertAsync(alert, cancellationToken);

        return alert;
    }

    public async Task<Alert> ResolveAsync(int id, ResolveAlertDto dto, CancellationToken cancellationToken = default)
    {
        var (alert, resolvedAt) = await validationService.ValidateResolveAsync(id, dto, cancellationToken);

        alert.IsResolved = true;
        alert.ResolvedAt = resolvedAt;

        alert = await alertRepository.UpdateAsync(alert, cancellationToken);

        return alert;
    }

    public async Task<PagedResult<Alert>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateAnimalAsync(animalId, cancellationToken);

        return await alertRepository.Query()
            .Where(x => x.AnimalId == animalId)
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
