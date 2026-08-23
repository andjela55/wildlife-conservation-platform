using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Services.Users;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper) : IUserService
{
    public async Task<PagedResult<User>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await userRepository.Query()
            .OrderBy(x => x.FullName)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<User> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var email = ServiceHelpers.RequiredText(dto.Email, nameof(dto.Email)).Trim().ToLowerInvariant();
        var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"User with email {email} already exists.");
        }

        var user = mapper.Map<User>(dto);
        ServiceHelpers.RequiredText(dto.FullName, nameof(dto.FullName));
        user.PasswordSalt = PasswordHasher.CreateSalt();
        user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);
        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);

        await ValidateRolesAsync(dto.RoleIds, cancellationToken);
        return await userRepository.InsertAsync(user, dto.RoleIds, cancellationToken);
    }

    public async Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"User with id {id} was not found.");

        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);
        mapper.Map(dto, user);

        return await userRepository.UpdateAsync(user, cancellationToken);
    }

    private async Task ValidateRolesAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken)
    {
        if (!await roleRepository.AllExistAsync(roleIds, cancellationToken))
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "At least one valid role must be assigned to the user.");
        }
    }

    public async Task<User> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"User with id {id} was not found.");
        var email = ServiceHelpers.RequiredText(dto.Email, nameof(dto.Email)).Trim().ToLowerInvariant();
        var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null && existingUser.Id != id)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"User with email {email} already exists.");
        }

        mapper.Map(dto, user);
        ServiceHelpers.RequiredText(dto.FullName, nameof(dto.FullName));
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordSalt = PasswordHasher.CreateSalt();
            user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);
        }
        ValidateAssignedArea(dto.AssignedLatitude, dto.AssignedLongitude);
        await ValidateRolesAsync(dto.RoleIds, cancellationToken);
        return await userRepository.UpdateAsync(user, dto.RoleIds, cancellationToken);
    }

    private static void ValidateAssignedArea(
        decimal? assignedLatitude,
        decimal? assignedLongitude)
    {
        if (assignedLatitude.HasValue != assignedLongitude.HasValue)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Assigned latitude and longitude must be set together.");
        }
    }
}
