namespace WildlifeConservation.Services.LocationPoints;

public class LocationPointService(
    ILocationPointRepository locationPointRepository,
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository,
    ICollarAssignmentRepository collarAssignmentRepository,
    ILocationPointNotificationService locationPointNotificationService,
    IMapper mapper) : ILocationPointService
{
    public async Task<LocationPoint> CreateAsync(CreateLocationPointDto dto, CancellationToken cancellationToken = default)
    {
        var animal = await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");
        var collar = await ServiceHelpers.EnsureFoundAsync(collarRepository.GetByIdAsync(dto.CollarId, cancellationToken), dto.CollarId, "Collar");

        var recordedAt = ServiceHelpers.AsUtc(dto.RecordedAt);
        var collarWasAssignedToAnimal = await collarAssignmentRepository.Query()
            .AnyAsync(x =>
                x.AnimalId == dto.AnimalId &&
                x.CollarId == dto.CollarId &&
                x.AssignedAt <= recordedAt &&
                (x.UnassignedAt == null || x.UnassignedAt >= recordedAt),
                cancellationToken);

        if (!collarWasAssignedToAnimal)
        {
            throw new ServiceException(
                (int)HttpStatusCode.BadRequest,
                "Location points can only be recorded from a collar assigned to the animal at the recorded time.");
        }

        var locationPoint = mapper.Map<LocationPoint>(dto);
        locationPoint.RecordedAt = recordedAt;
        locationPoint.Notes = dto.Notes?.Trim();

        locationPoint = await locationPointRepository.InsertAsync(locationPoint, cancellationToken);
        await locationPointNotificationService.NotifyLocationPointCreatedAsync(locationPoint, animal, collar, cancellationToken);

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
