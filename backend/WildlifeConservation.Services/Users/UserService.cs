using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Services.Users;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
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
        user.FullName = ServiceHelpers.RequiredText(dto.FullName, nameof(dto.FullName)).Trim();
        user.Email = email;
        user.PasswordSalt = PasswordHasher.CreateSalt();
        user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);
        ApplyAssignedArea(user, dto.AssignedLocationName, dto.AssignedLatitude, dto.AssignedLongitude, dto.AssignedMapZoom);

        return await userRepository.InsertAsync(user, cancellationToken);
    }

    public async Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"User with id {id} was not found.");

        ApplyAssignedArea(user, dto.AssignedLocationName, dto.AssignedLatitude, dto.AssignedLongitude, dto.AssignedMapZoom);

        return await userRepository.UpdateAsync(user, cancellationToken);
    }

    private static void ApplyAssignedArea(
        User user,
        string? assignedLocationName,
        decimal? assignedLatitude,
        decimal? assignedLongitude,
        int? assignedMapZoom)
    {
        if (assignedLatitude.HasValue != assignedLongitude.HasValue)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Assigned latitude and longitude must be set together.");
        }

        user.AssignedLocationName = string.IsNullOrWhiteSpace(assignedLocationName)
            ? null
            : assignedLocationName.Trim();
        user.AssignedLatitude = assignedLatitude;
        user.AssignedLongitude = assignedLongitude;
        user.AssignedMapZoom = assignedMapZoom;
    }
}
