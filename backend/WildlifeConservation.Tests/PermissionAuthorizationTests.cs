using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using WildlifeConservation.Api.Auth;
using WildlifeConservation.Api.Controllers;
using WildlifeConservation.Models.Users;
using WildlifeConservation.Repositories.Data;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Tests;

public class PermissionAuthorizationTests
{
    [Fact]
    public void PermissionCodeNumericValuesRemainStable()
    {
        Assert.Equal(0, (int)PermissionCode.AnimalsRead);
        Assert.Equal(1, (int)PermissionCode.AnimalsWrite);
        Assert.Equal(19, (int)PermissionCode.RolesWrite);
        Assert.Equal(20, (int)PermissionCode.Master);
    }

    [Fact]
    public void PermissionAttributeForwardsOneOrMorePermissions()
    {
        var attribute = new PermissionAttribute(PermissionCode.AnimalsRead, PermissionCode.AnimalsWrite);

        var permissions = Assert.IsType<PermissionCode[]>(Assert.Single(attribute.Arguments!));
        Assert.Equal(new[] { PermissionCode.AnimalsRead, PermissionCode.AnimalsWrite }, permissions);
    }

    [Fact]
    public async Task PermissionFilterAcceptsMasterForEveryProtectedEndpoint()
    {
        var authorizationService = new RecordingAuthorizationService();
        var filter = new PermissionFilter([PermissionCode.AnimalsRead], authorizationService);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "1")],
                authenticationType: "Test"))
        };
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());

        await filter.OnAuthorizationAsync(new AuthorizationFilterContext(actionContext, []));

        Assert.NotNull(authorizationService.Requirement);
        Assert.Contains(PermissionCode.AnimalsRead, authorizationService.Requirement.Permissions);
        Assert.Contains(PermissionCode.Master, authorizationService.Requirement.Permissions);
    }

    [Fact]
    public void PermissionSeedsGiveAdminExplicitRightsAndMasterOnlyTheMasterPermission()
    {
        var options = new DbContextOptionsBuilder<WildlifeDbContext>()
            .UseNpgsql("Host=localhost;Database=model-only")
            .Options;
        using var context = new WildlifeDbContext(options);
        var model = context.GetService<IDesignTimeModel>().Model;

        var permissions = model.FindEntityType(typeof(Permission))!.GetSeedData();
        var rolePermissions = model.FindEntityType(typeof(RolePermission))!.GetSeedData();
        var userRoles = model.FindEntityType(typeof(UserRole))!.GetSeedData();

        Assert.Contains(permissions, seed =>
            (int)seed[nameof(Permission.Id)]! == 21 &&
            (PermissionCode)seed[nameof(Permission.Code)]! == PermissionCode.Master);

        var adminPermissionIds = rolePermissions
            .Where(seed => (int)seed[nameof(RolePermission.RoleId)]! == 3)
            .Select(seed => (int)seed[nameof(RolePermission.PermissionId)]!)
            .Order()
            .ToArray();
        Assert.Equal(Enumerable.Range(1, 20), adminPermissionIds);

        var masterPermission = Assert.Single(rolePermissions,
            seed => (int)seed[nameof(RolePermission.RoleId)]! == 4);
        Assert.Equal(21, (int)masterPermission[nameof(RolePermission.PermissionId)]!);

        var administrativePermissionIds = new[] { 18, 20, 21 };
        Assert.DoesNotContain(rolePermissions, seed =>
            (int)seed[nameof(RolePermission.RoleId)]! is 1 or 2 &&
            administrativePermissionIds.Contains((int)seed[nameof(RolePermission.PermissionId)]!));

        Assert.Contains(userRoles, seed =>
            (int)seed[nameof(UserRole.UserId)]! == 4 &&
            (int)seed[nameof(UserRole.RoleId)]! == 4);
    }

    [Fact]
    public void EveryWriteEndpointDeclaresAWritePermission()
    {
        var unsecuredWrites = typeof(AnimalsController).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(IsWriteEndpoint)
            .Where(method => method.GetCustomAttribute<AllowAnonymousAttribute>() is null)
            .Where(method => !HasExplicitWriteAuthorization(method))
            .Select(method => $"{method.DeclaringType!.Name}.{method.Name}")
            .ToArray();

        Assert.True(unsecuredWrites.Length == 0,
            $"Write endpoints without an explicit write permission: {string.Join(", ", unsecuredWrites)}");
    }

    private static bool IsWriteEndpoint(MethodInfo method)
    {
        return method.GetCustomAttributes<HttpMethodAttribute>()
            .SelectMany(attribute => attribute.HttpMethods)
            .Any(httpMethod => httpMethod is "POST" or "PUT" or "PATCH" or "DELETE");
    }

    private static bool HasExplicitWriteAuthorization(MethodInfo method)
    {
        var permissions = method.GetCustomAttributes<PermissionAttribute>()
            .Concat(method.DeclaringType!.GetCustomAttributes<PermissionAttribute>())
            .SelectMany(attribute => (PermissionCode[])attribute.Arguments![0]);

        return permissions.Any(permission => permission.ToString().EndsWith("Write", StringComparison.Ordinal))
            || method.GetCustomAttribute<AuthorizeDeviceAttribute>() is not null;
    }

    private sealed class RecordingAuthorizationService : IAuthorizationService
    {
        public CurrentUserPermissionRequirement? Requirement { get; private set; }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            Requirement = Assert.IsType<CurrentUserPermissionRequirement>(Assert.Single(requirements));
            return Task.FromResult(AuthorizationResult.Success());
        }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            string policyName) => throw new NotSupportedException();
    }
}
