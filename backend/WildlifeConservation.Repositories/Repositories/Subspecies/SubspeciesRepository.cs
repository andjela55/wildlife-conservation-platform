using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Subspecies;

public interface ISubspeciesRepository
{
    IQueryable<Models.Subspecies.Subspecies> Query();
    Task<Models.Subspecies.Subspecies?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> InsertAsync(Models.Subspecies.Subspecies entity, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> UpdateAsync(Models.Subspecies.Subspecies entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Models.Subspecies.Subspecies entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(List<Models.Subspecies.Subspecies> entities, CancellationToken cancellationToken = default);
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> StartTransactionAsync(CancellationToken cancellationToken = default);
}

public class SubspeciesRepository(WildlifeDbContext dbContext)
    : BaseRepository<Models.Subspecies.Subspecies>(dbContext), ISubspeciesRepository
{
}
