using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WildlifeConservation.Api.Auth;

public sealed class PermissionFilter(
    PermissionCode[] permissions,
    IAuthorizationService authorizationService) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var principal = context.HttpContext.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var acceptedPermissions = permissions.Append(PermissionCode.Master).ToHashSet();
        var requirement = new CurrentUserPermissionRequirement(acceptedPermissions);
        var result = await authorizationService.AuthorizeAsync(principal, resource: null, requirement);
        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();
        }
    }
}
