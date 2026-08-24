namespace WildlifeConservation.Services.Subspecies;

public class SubspeciesService(
    ISubspeciesRepository subspeciesRepository,
    ISubspeciesValidationService validationService,
    IMapper mapper,
    IAnimalRepository animalRepository,
    IAlertRepository alertRepository,
    IRangerReportRepository rangerReportRepository,
    ILocationPointRepository locationPointRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : ISubspeciesService
{
    public async Task<PagedResult<Models.Subspecies.Subspecies>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await subspeciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Models.Subspecies.Subspecies> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Models.Subspecies.Subspecies> CreateAsync(UpsertSubspeciesDto dto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateUpsertAsync(dto, existingId: null, cancellationToken);

        var subspecies = mapper.Map<Models.Subspecies.Subspecies>(dto);

        subspecies = await subspeciesRepository.InsertAsync(subspecies, cancellationToken);

        return subspecies;
    }

    public async Task<Models.Subspecies.Subspecies> UpdateAsync(int id, UpsertSubspeciesDto dto, CancellationToken cancellationToken = default)
    {
        var subspecies = await validationService.GetRequiredAsync(id, cancellationToken);
        await validationService.ValidateUpsertAsync(dto, id, cancellationToken);

        mapper.Map(dto, subspecies);
        return await subspeciesRepository.UpdateAsync(subspecies, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var subspecies = await validationService.GetRequiredAsync(id, cancellationToken);
        var animalIds = await animalRepository.Query()
            .Where(x => x.SubspeciesId == id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        await using var transaction = await subspeciesRepository.StartTransactionAsync(cancellationToken);
        try
        {
            var alerts = await alertRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var reports = await rangerReportRepository.Query().Where(x => x.AnimalId.HasValue && animalIds.Contains(x.AnimalId.Value)).ToListAsync(cancellationToken);
            var locations = await locationPointRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var assignments = await collarAssignmentRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var animals = await animalRepository.Query().Where(x => animalIds.Contains(x.Id)).ToListAsync(cancellationToken);
            await alertRepository.DeleteRangeAsync(alerts, cancellationToken);
            await rangerReportRepository.DeleteRangeAsync(reports, cancellationToken);
            await locationPointRepository.DeleteRangeAsync(locations, cancellationToken);
            await collarAssignmentRepository.DeleteRangeAsync(assignments, cancellationToken);
            await animalRepository.DeleteRangeAsync(animals, cancellationToken);
            await subspeciesRepository.DeleteAsync(subspecies, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
