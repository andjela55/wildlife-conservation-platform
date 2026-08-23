namespace WildlifeConservation.Services.Species;

public class SpeciesService(ISpeciesRepository speciesRepository, IMapper mapper) : ISpeciesService
{
    public async Task<PagedResult<Models.Species.Species>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await speciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var species = await speciesRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return species is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Species with id {id} was not found.")
            : species;
    }

    public async Task<Models.Species.Species> CreateAsync(UpsertSpeciesDto dto, CancellationToken cancellationToken = default)
    {
        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));

        var duplicate = await speciesRepository.Query()
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);

        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Species '{name}' already exists.");
        }

        var species = mapper.Map<Models.Species.Species>(dto);

        species = await speciesRepository.InsertAsync(species, cancellationToken);

        return species;
    }

    public async Task<Models.Species.Species> UpdateAsync(int id, UpsertSpeciesDto dto, CancellationToken cancellationToken = default)
    {
        var species = await speciesRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Species with id {id} was not found.");
        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));

        var duplicate = await speciesRepository.Query()
            .AnyAsync(x => x.Id != id && x.Name.ToLower() == name.ToLower(), cancellationToken);
        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Species '{name}' already exists.");
        }

        mapper.Map(dto, species);
        return await speciesRepository.UpdateAsync(species, cancellationToken);
    }
}
