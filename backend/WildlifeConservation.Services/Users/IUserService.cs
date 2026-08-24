namespace WildlifeConservation.Services.Users;

public interface IUserService
{
    Task<PagedResult<User>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(CreateUserDto dto, int actorUserId, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(int id, UpdateUserDto dto, int actorUserId, CancellationToken cancellationToken = default);
    Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, int actorUserId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, int actorUserId, CancellationToken cancellationToken = default);
}
