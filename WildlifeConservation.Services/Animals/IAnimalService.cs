namespace WildlifeConservation.Services.Animals;

public interface IAnimalService
{
    Task<List<Animal>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Animal> CreateAsync(CreateAnimalDto dto, CancellationToken cancellationToken = default);
    Task<Animal> UpdateAsync(int id, UpdateAnimalDto dto, CancellationToken cancellationToken = default);
    Task<List<LocationPoint>> GetLocationsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RangerReport>> GetReportsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Alert>> GetAlertsAsync(int id, CancellationToken cancellationToken = default);
}
