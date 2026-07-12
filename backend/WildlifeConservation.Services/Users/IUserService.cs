namespace WildlifeConservation.Services.Users;

public interface IUserService
{
    Task<PagedResult<User>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, CancellationToken cancellationToken = default);
}
