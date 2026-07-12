using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.Auth;

public class AuthorizeRolesAttribute : TypeFilterAttribute
{
    public AuthorizeRolesAttribute(params UserRole[] roles)
        : base(typeof(AuthorizeRolesFilter))
    {
        Arguments = [roles];
    }
}

public class AuthorizeRolesFilter(UserRole[] roles, IAuthService authService) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
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
}
