using Microsoft.AspNetCore.Mvc;
using WildlifeConservation.Api.Auth;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IJwtTokenService jwtTokenService, IMapper mapper) : ControllerBase
{
    [HttpGet("current-user")]
    public async Task<ActionResult<CurrentUserResponseDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await authService.GetCurrentUserAsync(User.GetCurrentUserId(), cancellationToken);
        return Ok(mapper.Map<CurrentUserResponseDto>(user));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await authService.LoginAsync(dto, cancellationToken);
        return Ok(jwtTokenService.CreateToken(user));
    }
}
