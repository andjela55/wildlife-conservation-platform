namespace WildlifeConservation.Services.LocationPoints;

public class LocationPointService(
    ILocationPointRepository locationPointRepository,
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository,
    IMapper mapper) : ILocationPointService
{
    public async Task<LocationPoint> CreateAsync(CreateLocationPointDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");
        await ServiceHelpers.EnsureFoundAsync(collarRepository.GetByIdAsync(dto.CollarId, cancellationToken), dto.CollarId, "Collar");

        var locationPoint = mapper.Map<LocationPoint>(dto);
        locationPoint.RecordedAt = ServiceHelpers.AsUtc(dto.RecordedAt);
        locationPoint.Notes = dto.Notes?.Trim();

        locationPoint = await locationPointRepository.InsertAsync(locationPoint, cancellationToken);

        return locationPoint;
    }

    public async Task<List<LocationPoint>> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        var locations = await locationPointRepository.Query()
            .OrderByDescending(x => x.RecordedAt)
            .ToListAsync(cancellationToken);

        var latest = locations
            .GroupBy(x => x.AnimalId)
            .Select(x => x.First())
            .OrderBy(x => x.AnimalId)
            .ToList();

        return latest;
    }

    public async Task<List<LocationPoint>> GetByAnimalAsync(int animalId, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(animalId, cancellationToken), animalId, "Animal");

        var locations = await locationPointRepository.Query()
            .Where(x => x.AnimalId == animalId)
            .OrderByDescending(x => x.RecordedAt)
            .ToListAsync(cancellationToken);

        return locations;
    }
}
