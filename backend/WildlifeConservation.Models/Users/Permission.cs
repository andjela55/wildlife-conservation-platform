namespace WildlifeConservation.Models.Users;

public class Permission
{
    public int Id { get; set; }
    public PermissionCode Code { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
