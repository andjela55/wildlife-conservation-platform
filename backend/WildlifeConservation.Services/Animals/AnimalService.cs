namespace WildlifeConservation.Services.Animals;

public class AnimalService(
    IAnimalRepository animalRepository,
    IAnimalValidationService validationService,
    IMapper mapper,
    IAlertRepository alertRepository,
    IRangerReportRepository rangerReportRepository,
    ILocationPointRepository locationPointRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : IAnimalService
{
    public async Task<PagedResult<Animal>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await animalRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Animal> CreateAsync(UpsertAnimalDto dto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateUpsertAsync(dto, cancellationToken);

        var animal = mapper.Map<Animal>(dto);

        animal = await animalRepository.InsertAsync(animal, cancellationToken);

        return animal;
    }

    public async Task<Animal> UpdateAsync(int id, UpsertAnimalDto dto, CancellationToken cancellationToken = default)
    {
        var animal = await validationService.GetRequiredAsync(id, cancellationToken);
        await validationService.ValidateUpsertAsync(dto, cancellationToken);

        mapper.Map(dto, animal);

        animal = await animalRepository.UpdateAsync(animal, cancellationToken);

        return animal;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var animal = await validationService.GetRequiredAsync(id, cancellationToken);
        await using var transaction = await animalRepository.StartTransactionAsync(cancellationToken);
        try
        {
            var alerts = await alertRepository.Query().Where(x => x.AnimalId == id).ToListAsync(cancellationToken);
            var reports = await rangerReportRepository.Query().Where(x => x.AnimalId == id).ToListAsync(cancellationToken);
            var locations = await locationPointRepository.Query().Where(x => x.AnimalId == id).ToListAsync(cancellationToken);
            var assignments = await collarAssignmentRepository.Query().Where(x => x.AnimalId == id).ToListAsync(cancellationToken);
            await alertRepository.DeleteRangeAsync(alerts, cancellationToken);
            await rangerReportRepository.DeleteRangeAsync(reports, cancellationToken);
            await locationPointRepository.DeleteRangeAsync(locations, cancellationToken);
            await collarAssignmentRepository.DeleteRangeAsync(assignments, cancellationToken);
            await animalRepository.DeleteAsync(animal, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
