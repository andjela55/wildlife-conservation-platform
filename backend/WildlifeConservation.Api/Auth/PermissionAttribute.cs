using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(params PermissionCode[] permissions)
        : base(typeof(PermissionFilter))
    {
        if (permissions.Length == 0)
        {
            throw new ArgumentException("At least one permission is required.", nameof(permissions));
        }

        Arguments = [permissions];
    }
}
