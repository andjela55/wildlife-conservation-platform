namespace WildlifeConservation.Services.Animals;

public class AnimalService(
    IAnimalRepository animalRepository,
    ISubspeciesRepository subspeciesRepository,
    ILocationPointRepository locationPointRepository,
    IRangerReportRepository rangerReportRepository,
    IAlertRepository alertRepository,
    IMapper mapper) : IAnimalService
{
    public async Task<PagedResult<Animal>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await animalRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var animal = await animalRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return animal is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Animal with id {id} was not found.")
            : animal;
    }

    public async Task<Animal> CreateAsync(CreateAnimalDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(
            subspeciesRepository.GetByIdAsync(dto.SubspeciesId, cancellationToken),
            dto.SubspeciesId,
            "Subspecies");

        var animal = mapper.Map<Animal>(dto);
        animal.Name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        animal.DateOfBirth = ServiceHelpers.AsUtc(dto.DateOfBirth);
        animal.Notes = dto.Notes?.Trim();

        animal = await animalRepository.InsertAsync(animal, cancellationToken);

        return animal;
    }

    public async Task<Animal> UpdateAsync(int id, UpdateAnimalDto dto, CancellationToken cancellationToken = default)
    {
        var animal = await animalRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Animal with id {id} was not found.");

        await ServiceHelpers.EnsureFoundAsync(
            subspeciesRepository.GetByIdAsync(dto.SubspeciesId, cancellationToken),
            dto.SubspeciesId,
            "Subspecies");

        mapper.Map(dto, animal);
        animal.Name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        animal.DateOfBirth = ServiceHelpers.AsUtc(dto.DateOfBirth);
        animal.Notes = dto.Notes?.Trim();

        animal = await animalRepository.UpdateAsync(animal, cancellationToken);

        return animal;
    }

    public async Task<PagedResult<LocationPoint>> GetLocationsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(id, cancellationToken), id, "Animal");

        return await locationPointRepository.Query()
            .Where(x => x.AnimalId == id)
            .OrderByDescending(x => x.RecordedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<PagedResult<RangerReport>> GetReportsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(id, cancellationToken), id, "Animal");

        return await rangerReportRepository.Query()
            .Where(x => x.AnimalId == id)
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<PagedResult<Alert>> GetAlertsAsync(int id, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(id, cancellationToken), id, "Animal");

        return await alertRepository.Query()
            .Where(x => x.AnimalId == id)
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
