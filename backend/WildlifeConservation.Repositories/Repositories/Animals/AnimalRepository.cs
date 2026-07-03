using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Animals;

public interface IAnimalRepository
{
    IQueryable<Animal> Query();
    Task<Animal?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Animal> InsertAsync(Animal entity, CancellationToken cancellationToken = default);
    Task<Animal> UpdateAsync(Animal entity, CancellationToken cancellationToken = default);
}

public class AnimalRepository(WildlifeDbContext dbContext)
    : BaseRepository<Animal>(dbContext), IAnimalRepository
{
}
