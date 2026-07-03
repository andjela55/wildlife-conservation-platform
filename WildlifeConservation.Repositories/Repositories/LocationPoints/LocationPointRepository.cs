using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.LocationPoints;

public interface ILocationPointRepository
{
    IQueryable<LocationPoint> Query();
    Task<LocationPoint> InsertAsync(LocationPoint entity, CancellationToken cancellationToken = default);
}

public class LocationPointRepository(WildlifeDbContext dbContext)
    : BaseRepository<LocationPoint>(dbContext), ILocationPointRepository
{
}
