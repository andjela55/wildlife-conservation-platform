using System.Security.Claims;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetCurrentUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userId, out var id)
            ? id
            : throw new ServiceException(StatusCodes.Status401Unauthorized, "Authenticated user id is missing.");
    }
}
