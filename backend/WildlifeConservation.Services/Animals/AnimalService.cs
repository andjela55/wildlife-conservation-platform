namespace WildlifeConservation.Services.Animals;

public class AnimalService(
    IAnimalRepository animalRepository,
    ISubspeciesRepository subspeciesRepository,
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

    public async Task<Animal> CreateAsync(UpsertAnimalDto dto, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(
            subspeciesRepository.GetByIdAsync(dto.SubspeciesId, cancellationToken),
            dto.SubspeciesId,
            "Subspecies");

        var animal = mapper.Map<Animal>(dto);
        ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));

        animal = await animalRepository.InsertAsync(animal, cancellationToken);

        return animal;
    }

    public async Task<Animal> UpdateAsync(int id, UpsertAnimalDto dto, CancellationToken cancellationToken = default)
    {
        var animal = await animalRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Animal with id {id} was not found.");

        await ServiceHelpers.EnsureFoundAsync(
            subspeciesRepository.GetByIdAsync(dto.SubspeciesId, cancellationToken),
            dto.SubspeciesId,
            "Subspecies");

        mapper.Map(dto, animal);
        ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));

        animal = await animalRepository.UpdateAsync(animal, cancellationToken);

        return animal;
    }

}
