namespace WildlifeConservation.Services.Auth;

public interface IAuthService
{
    Task<User> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<User> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
}
