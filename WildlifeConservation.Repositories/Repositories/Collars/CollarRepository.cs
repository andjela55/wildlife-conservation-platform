using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Collars;

public interface ICollarRepository
{
    IQueryable<Collar> Query();
    Task<Collar?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Collar> InsertAsync(Collar entity, CancellationToken cancellationToken = default);
    Task<Collar> UpdateAsync(Collar entity, CancellationToken cancellationToken = default);
}

public class CollarRepository(WildlifeDbContext dbContext)
    : BaseRepository<Collar>(dbContext), ICollarRepository
{
}
