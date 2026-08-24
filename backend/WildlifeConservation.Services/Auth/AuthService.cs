namespace WildlifeConservation.Services.Auth;

public class AuthService(IAuthValidationService validationService) : IAuthService
{
    public async Task<User> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default) =>
        await validationService.ValidateCurrentUserAsync(userId, cancellationToken);

    public async Task<User> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default) =>
        await validationService.ValidateLoginAsync(dto, cancellationToken);
}
