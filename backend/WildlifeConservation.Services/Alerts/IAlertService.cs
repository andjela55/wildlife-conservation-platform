namespace WildlifeConservation.Services.Alerts;

public interface IAlertService
{
    Task<PagedResult<Alert>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Alert> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Alert> CreateAsync(CreateAlertDto dto, int createdByUserId, CancellationToken cancellationToken = default);
    Task<Alert> ResolveAsync(int id, ResolveAlertDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<Alert>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default);
}
