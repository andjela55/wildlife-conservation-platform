using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WildlifeConservation.Api.Auth;

public sealed class AuthorizeDeviceAttribute : TypeFilterAttribute
{
    public AuthorizeDeviceAttribute()
        : base(typeof(AuthorizeDeviceFilter))
    {
    }
}

public sealed class AuthorizeDeviceFilter(IConfiguration configuration) : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!HasValidDeviceKey(context.HttpContext.Request, configuration["DeviceApiKey"]))
        {
            context.Result = new UnauthorizedResult();
        }

        return Task.CompletedTask;
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