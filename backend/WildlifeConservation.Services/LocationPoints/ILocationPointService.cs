namespace WildlifeConservation.Services.LocationPoints;

public interface ILocationPointService
{
    Task<LocationPoint> CreateAsync(CreateLocationPointDto dto, CancellationToken cancellationToken = default);
    Task<List<LocationPoint>> GetLatestAsync(CancellationToken cancellationToken = default);
    Task<List<LocationPoint>> GetByAnimalAsync(int animalId, CancellationToken cancellationToken = default);
}
