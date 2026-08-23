using WildlifeConservation.Models.Users;
using WildlifeConservation.Services.Users;
using WildlifeConservation.Shared;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Tests;

public class UserValidationServiceTests
{
    private readonly IUserValidationService validationService = new UserValidationService(null!, null!);

    [Fact]
    public void MasterCanModifyAnyUserAndAssignAnyRole()
    {
        var master = UserWithPermissions(1, PermissionCode.Master);
        var admin = UserWithPermissions(2, PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var masterRole = RoleWithPermissions(PermissionCode.Master);

        validationService.EnsureCanModify(master, admin);
        validationService.EnsureCanAssignRoles(master, [masterRole], isSelf: false);
    }

    [Fact]
    public void AdminCanModifyRegularUsersAndThemselves()
    {
        var admin = UserWithPermissions(1, PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var regularUser = UserWithPermissions(2, PermissionCode.AnimalsRead);

        validationService.EnsureCanModify(admin, regularUser);
        validationService.EnsureCanModify(admin, admin);
    }

    [Fact]
    public void AdminCannotModifyAnotherAdminOrMaster()
    {
        var admin = UserWithPermissions(1, PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var otherAdmin = UserWithPermissions(2, PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var master = UserWithPermissions(3, PermissionCode.Master);

        AssertForbidden(() => validationService.EnsureCanModify(admin, otherAdmin));
        AssertForbidden(() => validationService.EnsureCanModify(admin, master));
    }

    [Fact]
    public void AdminCannotGrantAdministrativeAccessToOtherUsersOrGrantMasterToThemself()
    {
        var admin = UserWithPermissions(1, PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var adminRole = RoleWithPermissions(PermissionCode.UsersWrite, PermissionCode.RolesWrite);
        var masterRole = RoleWithPermissions(PermissionCode.Master);

        AssertForbidden(() => validationService.EnsureCanAssignRoles(admin, [adminRole], isSelf: false));
        AssertForbidden(() => validationService.EnsureCanAssignRoles(admin, [masterRole], isSelf: true));
        validationService.EnsureCanAssignRoles(admin, [adminRole], isSelf: true);
    }

    private static User UserWithPermissions(int id, params PermissionCode[] permissions) => new()
    {
        Id = id,
        UserRoles =
        [
            new UserRole
            {
                Role = RoleWithPermissions(permissions)
            }
        ]
    };

    private static Role RoleWithPermissions(params PermissionCode[] permissions) => new()
    {
        RolePermissions = permissions
            .Select(permission => new RolePermission
            {
                Permission = new Permission { Code = permission }
            })
            .ToList()
    };

    private static void AssertForbidden(Action action)
    {
        var exception = Assert.Throws<ServiceException>(action);
        Assert.Equal(403, exception.StatusCode);
    }
}
