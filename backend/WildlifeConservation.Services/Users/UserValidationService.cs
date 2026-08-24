namespace WildlifeConservation.Services.Users;

public class UserValidationService(IUserRepository userRepository, IRoleRepository roleRepository) : IUserValidationService
{
    public async Task ValidateCreateAsync(CreateUserDto dto, int actorUserId, CancellationToken cancellationToken)
    {
        var actor = await GetActorAsync(actorUserId, cancellationToken);
        var roles = await GetValidRolesAsync(dto.RoleIds, cancellationToken);
        EnsureCanAssignRoles(actor, roles, isSelf: false);
        await ValidateIdentityAsync(dto.FullName, dto.Email, existingUserId: null, cancellationToken);
        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);
    }

    public async Task<User> ValidateAssignedAreaUpdateAsync(
        int userId,
        UpdateUserAssignedAreaDto dto,
        int actorUserId,
        CancellationToken cancellationToken)
    {
        var actor = await GetActorAsync(actorUserId, cancellationToken);
        var user = await GetUserAsync(userId, cancellationToken);
        EnsureCanModify(actor, user);
        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);
        return user;
    }

    public async Task<User> ValidateUpdateAsync(
        int userId,
        UpdateUserDto dto,
        int actorUserId,
        CancellationToken cancellationToken)
    {
        var actor = await GetActorAsync(actorUserId, cancellationToken);
        var user = await GetUserAsync(userId, cancellationToken);
        EnsureCanModify(actor, user);
        var roles = await GetValidRolesAsync(dto.RoleIds, cancellationToken);
        EnsureCanAssignRoles(actor, roles, actor.Id == user.Id);
        await ValidateIdentityAsync(dto.FullName, dto.Email, userId, cancellationToken);
        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);
        return user;
    }

    public async Task<User> ValidateDeleteAsync(int userId, int actorUserId, CancellationToken cancellationToken)
    {
        var actor = await GetActorAsync(actorUserId, cancellationToken);
        var user = await GetUserAsync(userId, cancellationToken);
        EnsureCanModify(actor, user);
        if (actor.Id == user.Id)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "You cannot delete your own user account.");
        }

        return user;
    }

    public void EnsureCanModify(User actor, User target)
    {
        if (HasPermission(actor, PermissionCode.Master) || actor.Id == target.Id)
        {
            return;
        }

        if (IsAdministrative(target))
        {
            throw Forbidden("Only a Master user can modify another Admin or Master user.");
        }
    }

    public void EnsureCanAssignRoles(User actor, IReadOnlyCollection<Role> roles, bool isSelf)
    {
        if (HasPermission(actor, PermissionCode.Master))
        {
            return;
        }

        var permissions = roles.SelectMany(role => role.RolePermissions)
            .Select(rolePermission => rolePermission.Permission.Code)
            .ToHashSet();
        if (permissions.Contains(PermissionCode.Master))
        {
            throw Forbidden("Only a Master user can assign the Master role.");
        }

        var assignsAdministrativeAccess = permissions.Contains(PermissionCode.UsersWrite)
            && permissions.Contains(PermissionCode.RolesWrite);
        if (assignsAdministrativeAccess && (!isSelf || !IsAdmin(actor)))
        {
            throw Forbidden("An Admin can assign the Admin role only to their own account.");
        }
    }

    private async Task ValidateIdentityAsync(string fullName, string emailValue, int? existingUserId, CancellationToken cancellationToken)
    {
        ServiceHelpers.RequiredText(fullName, nameof(fullName));
        var email = ServiceHelpers.RequiredText(emailValue, nameof(emailValue)).Trim().ToLowerInvariant();
        var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null && existingUser.Id != existingUserId)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"User with email {email} already exists.");
        }
    }

    private async Task<IReadOnlyCollection<Role>> GetValidRolesAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken)
    {
        var distinctRoleIds = roleIds.Distinct().ToArray();
        var roles = await roleRepository.GetByIdsAsync(distinctRoleIds, cancellationToken);
        if (distinctRoleIds.Length == 0 || roles.Count != distinctRoleIds.Length)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "At least one valid role must be assigned to the user.");
        }

        return roles;
    }

    private async Task<User> GetActorAsync(int actorUserId, CancellationToken cancellationToken) =>
        await userRepository.GetByIdAsync(actorUserId, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.Unauthorized, "Current user was not found.");

    private async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken) =>
        await userRepository.GetByIdAsync(userId, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"User with id {userId} was not found.");

    private static void ValidateAssignedArea(decimal? latitude, decimal? longitude)
    {
        if (latitude.HasValue != longitude.HasValue)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Assigned latitude and longitude must be set together.");
        }
    }

    private static bool IsAdministrative(User user) => HasPermission(user, PermissionCode.Master) || IsAdmin(user);
    private static bool IsAdmin(User user) => HasPermission(user, PermissionCode.UsersWrite) && HasPermission(user, PermissionCode.RolesWrite);
    private static bool HasPermission(User user, PermissionCode permission) =>
        user.UserRoles.Any(userRole => userRole.Role.RolePermissions.Any(rolePermission => rolePermission.Permission.Code == permission));
    private static ServiceException Forbidden(string message) => new((int)HttpStatusCode.Forbidden, message);
}
