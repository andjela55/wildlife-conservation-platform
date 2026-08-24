namespace WildlifeConservation.Services.Species;

public class SpeciesService(
    ISpeciesRepository speciesRepository,
    ISpeciesValidationService validationService,
    IMapper mapper,
    ISubspeciesRepository subspeciesRepository,
    IAnimalRepository animalRepository,
    IAlertRepository alertRepository,
    IRangerReportRepository rangerReportRepository,
    ILocationPointRepository locationPointRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : ISpeciesService
{
    public async Task<PagedResult<Models.Species.Species>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await speciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Models.Species.Species> CreateAsync(UpsertSpeciesDto dto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateUpsertAsync(dto, existingId: null, cancellationToken);

        var species = mapper.Map<Models.Species.Species>(dto);

        species = await speciesRepository.InsertAsync(species, cancellationToken);

        return species;
    }

    public async Task<Models.Species.Species> UpdateAsync(int id, UpsertSpeciesDto dto, CancellationToken cancellationToken = default)
    {
        var species = await validationService.GetRequiredAsync(id, cancellationToken);
        await validationService.ValidateUpsertAsync(dto, id, cancellationToken);

        mapper.Map(dto, species);
        return await speciesRepository.UpdateAsync(species, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var species = await validationService.GetRequiredAsync(id, cancellationToken);
        var subspeciesIds = await subspeciesRepository.Query()
            .Where(x => x.SpeciesId == id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var animalIds = await animalRepository.Query()
            .Where(x => subspeciesIds.Contains(x.SubspeciesId))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        await using var transaction = await speciesRepository.StartTransactionAsync(cancellationToken);
        try
        {
            var alerts = await alertRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var reports = await rangerReportRepository.Query().Where(x => x.AnimalId.HasValue && animalIds.Contains(x.AnimalId.Value)).ToListAsync(cancellationToken);
            var locations = await locationPointRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var assignments = await collarAssignmentRepository.Query().Where(x => animalIds.Contains(x.AnimalId)).ToListAsync(cancellationToken);
            var animals = await animalRepository.Query().Where(x => animalIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var subspecies = await subspeciesRepository.Query().Where(x => subspeciesIds.Contains(x.Id)).ToListAsync(cancellationToken);
            await alertRepository.DeleteRangeAsync(alerts, cancellationToken);
            await rangerReportRepository.DeleteRangeAsync(reports, cancellationToken);
            await locationPointRepository.DeleteRangeAsync(locations, cancellationToken);
            await collarAssignmentRepository.DeleteRangeAsync(assignments, cancellationToken);
            await animalRepository.DeleteRangeAsync(animals, cancellationToken);
            await subspeciesRepository.DeleteRangeAsync(subspecies, cancellationToken);
            await speciesRepository.DeleteAsync(species, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
