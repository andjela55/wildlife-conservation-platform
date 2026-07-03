namespace WildlifeConservation.Services.Species;

public class SpeciesService(ISpeciesRepository speciesRepository, IMapper mapper) : ISpeciesService
{
    public async Task<List<Models.Species.Species>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var species = await speciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return species;
    }

    public async Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var species = await speciesRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return species is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Species with id {id} was not found.")
            : species;
    }

    public async Task<Models.Species.Species> CreateAsync(CreateSpeciesDto dto, CancellationToken cancellationToken = default)
    {
        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        var description = ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));

        var duplicate = await speciesRepository.Query()
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);

        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Species '{name}' already exists.");
        }

        var species = mapper.Map<Models.Species.Species>(dto);
        species.Name = name;
        species.Description = description;

        species = await speciesRepository.InsertAsync(species, cancellationToken);

        return species;
    }
}
