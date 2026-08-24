using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.RangerReports;

public interface IRangerReportRepository
{
    IQueryable<RangerReport> Query();
    Task<RangerReport> InsertAsync(RangerReport entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(List<RangerReport> entities, CancellationToken cancellationToken = default);
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> StartTransactionAsync(CancellationToken cancellationToken = default);
}

public class RangerReportRepository(WildlifeDbContext dbContext)
    : BaseRepository<RangerReport>(dbContext), IRangerReportRepository
{
}
