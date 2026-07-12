using WildlifeConservation.Models.Users;

namespace WildlifeConservation.Api.Auth;

public interface IJwtTokenService
{
    LoginResponseDto CreateToken(User user);
}
