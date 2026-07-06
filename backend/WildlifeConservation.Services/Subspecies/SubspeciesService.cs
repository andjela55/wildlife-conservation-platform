namespace WildlifeConservation.Services.Subspecies;

public class SubspeciesService(
    ISubspeciesRepository subspeciesRepository,
    ISpeciesRepository speciesRepository,
    IMapper mapper) : ISubspeciesService
{
    public async Task<PagedResult<Models.Subspecies.Subspecies>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await subspeciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Models.Subspecies.Subspecies> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var subspecies = await subspeciesRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return subspecies is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Subspecies with id {id} was not found.")
            : subspecies;
    }

    public async Task<Models.Subspecies.Subspecies> CreateAsync(CreateSubspeciesDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(speciesRepository.GetByIdAsync(dto.SpeciesId, cancellationToken), dto.SpeciesId, "Species");

        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        var description = ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));

        var duplicate = await subspeciesRepository.Query()
            .AnyAsync(x => x.SpeciesId == dto.SpeciesId && x.Name.ToLower() == name.ToLower(), cancellationToken);

        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Subspecies '{name}' already exists for this species.");
        }

        var subspecies = mapper.Map<Models.Subspecies.Subspecies>(dto);
        subspecies.Name = name;
        subspecies.Description = description;

        subspecies = await subspeciesRepository.InsertAsync(subspecies, cancellationToken);

        return subspecies;
    }
}
