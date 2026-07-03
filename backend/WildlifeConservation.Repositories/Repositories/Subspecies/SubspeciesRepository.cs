using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Subspecies;

public interface ISubspeciesRepository
{
    IQueryable<Models.Subspecies.Subspecies> Query();
    Task<Models.Subspecies.Subspecies?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> InsertAsync(Models.Subspecies.Subspecies entity, CancellationToken cancellationToken = default);
}

public class SubspeciesRepository(WildlifeDbContext dbContext)
    : BaseRepository<Models.Subspecies.Subspecies>(dbContext), ISubspeciesRepository
{
}
