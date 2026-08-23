namespace WildlifeConservation.Services.Auth;

public interface IAuthValidationService
{
    Task<User> ValidateCurrentUserAsync(int userId, CancellationToken cancellationToken);
    Task<User> ValidateLoginAsync(LoginDto dto, CancellationToken cancellationToken);
}
