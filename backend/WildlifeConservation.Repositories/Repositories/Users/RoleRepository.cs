using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Users;

public interface IRoleRepository
{
    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> AllExistAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);
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

    public async Task<bool> AllExistAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToArray();
        return distinctIds.Length > 0 && await dbContext.Roles.CountAsync(x => distinctIds.Contains(x.Id), cancellationToken) == distinctIds.Length;
    }
}
