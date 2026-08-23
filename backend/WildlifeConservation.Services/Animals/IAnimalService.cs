namespace WildlifeConservation.Services.Animals;

public interface IAnimalService
{
    Task<PagedResult<Animal>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Animal> CreateAsync(UpsertAnimalDto dto, CancellationToken cancellationToken = default);
    Task<Animal> UpdateAsync(int id, UpsertAnimalDto dto, CancellationToken cancellationToken = default);
}
