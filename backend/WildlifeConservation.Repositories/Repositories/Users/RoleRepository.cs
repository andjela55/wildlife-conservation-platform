using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Users;

public interface IRoleRepository
{
    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Role>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);
}

public class RoleRepository(WildlifeDbContext dbContext) : IRoleRepository
{
    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Roles
            .AsNoTracking()
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Role>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToArray();
        return await dbContext.Roles
            .AsNoTracking()
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .Where(x => distinctIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
