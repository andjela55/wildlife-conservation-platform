namespace WildlifeConservation.Services.Users;

public interface IRoleService
{
    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);
}
