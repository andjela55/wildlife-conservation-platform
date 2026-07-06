namespace WildlifeConservation.Services.LocationPoints;

public interface ILocationPointService
{
    Task<LocationPoint> CreateAsync(CreateLocationPointDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<LocationPoint>> GetLatestAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<PagedResult<LocationPoint>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default);
}
