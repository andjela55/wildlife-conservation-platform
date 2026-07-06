namespace WildlifeConservation.Services.Animals;

public interface IAnimalService
{
    Task<PagedResult<Animal>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Animal> CreateAsync(CreateAnimalDto dto, CancellationToken cancellationToken = default);
    Task<Animal> UpdateAsync(int id, UpdateAnimalDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<LocationPoint>> GetLocationsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<PagedResult<RangerReport>> GetReportsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<PagedResult<Alert>> GetAlertsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default);
}
