namespace WildlifeConservation.Services.Animals;

public class AnimalService(
    IAnimalRepository animalRepository,
    IAnimalValidationService validationService,
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

}
