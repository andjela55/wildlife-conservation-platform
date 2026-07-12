using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Services.Auth;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    public async Task<User> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        return user is null || !user.IsActive
            ? throw new ServiceException((int)HttpStatusCode.Unauthorized, "Current user was not found or is inactive.")
            : user;
    }

    public async Task<User> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var email = ServiceHelpers.RequiredText(dto.Email, nameof(dto.Email)).Trim();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordSalt, user.PasswordHash))
        {
            throw new ServiceException((int)HttpStatusCode.Unauthorized, "Invalid email or password.");
        }

        return user;
    }
}
