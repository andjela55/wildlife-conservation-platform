namespace WildlifeConservation.Services.Alerts;

public interface IAlertService
{
    Task<List<Alert>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Alert> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Alert> CreateAsync(CreateAlertDto dto, CancellationToken cancellationToken = default);
    Task<Alert> ResolveAsync(int id, ResolveAlertDto dto, CancellationToken cancellationToken = default);
}
