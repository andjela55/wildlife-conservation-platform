using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.Auth;

public class AuthorizeDeviceOrRolesAttribute : TypeFilterAttribute
{
    public AuthorizeDeviceOrRolesAttribute(params UserRole[] roles)
        : base(typeof(AuthorizeDeviceOrRolesFilter))
    {
        Arguments = [roles];
    }
}

public class AuthorizeDeviceOrRolesFilter(
    UserRole[] roles,
    IAuthService authService,
    IConfiguration configuration) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (HasValidDeviceKey(context.HttpContext.Request, configuration["DeviceApiKey"]))
        {
            return;
        }

        var principal = context.HttpContext.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        int userId;
        try
        {
            userId = principal.GetCurrentUserId();
        }
        catch
        {
            context.Result = new ChallengeResult();
            return;
        }

        var user = await authService.GetCurrentUserAsync(userId, context.HttpContext.RequestAborted);
        if (user.Role != UserRole.Master && roles.Length > 0 && !roles.Contains(user.Role))
        {
            context.Result = new ForbidResult();
        }
    }

    private static bool HasValidDeviceKey(HttpRequest request, string? configuredKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey)
            || !request.Headers.TryGetValue("X-Device-Key", out var suppliedValues))
        {
            return false;
        }

        var suppliedKey = suppliedValues.ToString();
        if (string.IsNullOrWhiteSpace(suppliedKey))
        {
            return false;
        }

        var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey));
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(suppliedKey));
        return CryptographicOperations.FixedTimeEquals(configuredHash, suppliedHash);
    }
}

