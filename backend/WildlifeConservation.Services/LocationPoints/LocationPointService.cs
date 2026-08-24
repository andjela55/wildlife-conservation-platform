namespace WildlifeConservation.Services.LocationPoints;

public class LocationPointService(
    ILocationPointRepository locationPointRepository,
    ILocationPointValidationService validationService,
    ILocationPointNotificationService locationPointNotificationService,
    IMapper mapper) : ILocationPointService
{
    public async Task<LocationPoint> CreateAsync(CreateLocationPointDto dto, CancellationToken cancellationToken = default)
    {
        var (animal, collar) = await validationService.ValidateCreateAsync(dto, cancellationToken);

        var locationPoint = mapper.Map<LocationPoint>(dto);

        locationPoint = await locationPointRepository.InsertAsync(locationPoint, cancellationToken);
        await locationPointNotificationService.NotifyLocationPointCreatedAsync(locationPoint, animal, collar, cancellationToken);

        return locationPoint;
    }

    public async Task<PagedResult<LocationPoint>> GetLatestAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        var locations = await locationPointRepository.Query()
            .OrderByDescending(x => x.RecordedAt)
            .ToListAsync(cancellationToken);

        var latest = locations
            .GroupBy(x => x.AnimalId)
            .Select(x => x.First())
            .OrderBy(x => x.AnimalId)
            .ToList();

        return latest.ToPagedResult(pagination);
    }

    public async Task<PagedResult<LocationPoint>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateAnimalAsync(animalId, cancellationToken);

        return await locationPointRepository.Query()
            .Where(x => x.AnimalId == animalId)
            .OrderByDescending(x => x.RecordedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
