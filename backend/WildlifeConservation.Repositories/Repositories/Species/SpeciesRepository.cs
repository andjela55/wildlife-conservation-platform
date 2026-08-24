using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Species;

public interface ISpeciesRepository
{
    IQueryable<Models.Species.Species> Query();
    Task<Models.Species.Species?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> InsertAsync(Models.Species.Species entity, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> UpdateAsync(Models.Species.Species entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Models.Species.Species entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(List<Models.Species.Species> entities, CancellationToken cancellationToken = default);
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> StartTransactionAsync(CancellationToken cancellationToken = default);
}

public class SpeciesRepository(WildlifeDbContext dbContext)
    : BaseRepository<Models.Species.Species>(dbContext), ISpeciesRepository
{
}
