namespace WildlifeConservation.Services.Users;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        roleRepository.GetAllAsync(cancellationToken);
}
