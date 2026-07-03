using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Alerts;

public interface IAlertRepository
{
    IQueryable<Alert> Query();
    Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Alert> InsertAsync(Alert entity, CancellationToken cancellationToken = default);
    Task<Alert> UpdateAsync(Alert entity, CancellationToken cancellationToken = default);
}

public class AlertRepository(WildlifeDbContext dbContext)
    : BaseRepository<Alert>(dbContext), IAlertRepository
{
}
