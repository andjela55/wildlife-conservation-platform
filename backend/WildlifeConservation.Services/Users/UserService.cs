using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Services.Users;

public class UserService(IUserRepository userRepository, IUserValidationService validationService, IMapper mapper) : IUserService
{
    public async Task<PagedResult<User>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await userRepository.Query()
            .OrderBy(x => x.FullName)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<User> CreateAsync(CreateUserDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateCreateAsync(dto, actorUserId, cancellationToken);

        var user = mapper.Map<User>(dto);
        user.PasswordSalt = PasswordHasher.CreateSalt();
        user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);

        return await userRepository.InsertAsync(user, dto.RoleIds, cancellationToken);
    }

    public async Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        var user = await validationService.ValidateAssignedAreaUpdateAsync(id, dto, actorUserId, cancellationToken);
        mapper.Map(dto, user);

        return await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task<User> UpdateAsync(int id, UpdateUserDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        var user = await validationService.ValidateUpdateAsync(id, dto, actorUserId, cancellationToken);

        mapper.Map(dto, user);
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordSalt = PasswordHasher.CreateSalt();
            user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);
        }
        return await userRepository.UpdateAsync(user, dto.RoleIds, cancellationToken);
    }
}
