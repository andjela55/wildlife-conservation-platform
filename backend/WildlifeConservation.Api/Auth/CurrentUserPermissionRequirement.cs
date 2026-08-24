using Microsoft.AspNetCore.Authorization;
using WildlifeConservation.Repositories.Repositories.Users;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Api.Auth;

public sealed record CurrentUserPermissionRequirement(IReadOnlySet<PermissionCode> Permissions) : IAuthorizationRequirement;

public sealed class CurrentUserPermissionAuthorizationHandler(IUserRepository userRepository)
    : AuthorizationHandler<CurrentUserPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CurrentUserPermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        try
        {
            if (await userRepository.HasAnyPermissionAsync(context.User.GetCurrentUserId(), requirement.Permissions))
            {
                context.Succeed(requirement);
            }
        }
        catch (ServiceException)
        {
            // An invalid token identity is not authorized.
        }
    }
}
