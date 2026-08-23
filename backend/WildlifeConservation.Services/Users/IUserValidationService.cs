namespace WildlifeConservation.Services.Users;

public interface IUserValidationService
{
    Task ValidateCreateAsync(CreateUserDto dto, int actorUserId, CancellationToken cancellationToken);
    Task<User> ValidateAssignedAreaUpdateAsync(
        int userId,
        UpdateUserAssignedAreaDto dto,
        int actorUserId,
        CancellationToken cancellationToken);
    Task<User> ValidateUpdateAsync(
        int userId,
        UpdateUserDto dto,
        int actorUserId,
        CancellationToken cancellationToken);
    void EnsureCanModify(User actor, User target);
    void EnsureCanAssignRoles(User actor, IReadOnlyCollection<Role> roles, bool isSelf);
}
