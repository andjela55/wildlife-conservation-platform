using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.CollarAssignments;

public interface ICollarAssignmentRepository
{
    IQueryable<CollarAssignment> Query();
    Task<CollarAssignment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CollarAssignment> InsertAsync(CollarAssignment entity, CancellationToken cancellationToken = default);
    Task<CollarAssignment> UpdateAsync(CollarAssignment entity, CancellationToken cancellationToken = default);
}

public class CollarAssignmentRepository(WildlifeDbContext dbContext)
    : BaseRepository<CollarAssignment>(dbContext), ICollarAssignmentRepository
{
}
