namespace WildlifeConservation.Services.Species;

public class SpeciesService(ISpeciesRepository speciesRepository, ISpeciesValidationService validationService, IMapper mapper) : ISpeciesService
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
}
