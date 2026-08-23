using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Repositories.Data;
using PermissionCode = WildlifeConservation.Shared.Enums.PermissionCode;

namespace WildlifeConservation.Repositories.Repositories.Users;

public interface IUserRepository
{
    IQueryable<User> Query();
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> InsertAsync(User entity, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User entity, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default);
    Task<bool> HasAnyPermissionAsync(int userId, IReadOnlyCollection<PermissionCode> permissionCodes, CancellationToken cancellationToken = default);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(WildlifeDbContext dbContext) : base(dbContext)
    {
    }

    public new IQueryable<User> Query() => DbSet
        .AsNoTracking()
        .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission);

    public new async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User> InsertAsync(User entity, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default)
    {
        entity.UserRoles = roleIds.Distinct().Select(roleId => new UserRole { RoleId = roleId }).ToList();
        DbSet.Add(entity);
        await SaveChangesAsync(cancellationToken);
        ClearTracking();
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public new async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task<User> UpdateAsync(User entity, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default)
    {
        var userRoleSet = Set<UserRole>();
        var existingAssignments = await userRoleSet.Where(x => x.UserId == entity.Id).ToListAsync(cancellationToken);
        userRoleSet.RemoveRange(existingAssignments);
        userRoleSet.AddRange(roleIds.Distinct().Select(roleId => new UserRole { UserId = entity.Id, RoleId = roleId }));

        await base.UpdateAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        ClearTracking();
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public Task<bool> HasAnyPermissionAsync(int userId, IReadOnlyCollection<PermissionCode> permissionCodes, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(user =>
            user.Id == userId &&
            user.IsActive &&
            user.UserRoles.Any(userRole => userRole.Role.RolePermissions.Any(rolePermission =>
                permissionCodes.Contains(rolePermission.Permission.Code))), cancellationToken);
}
