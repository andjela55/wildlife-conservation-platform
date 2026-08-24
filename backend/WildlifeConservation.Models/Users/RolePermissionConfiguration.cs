using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Users;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => new { x.RoleId, x.PermissionId });
        builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 },
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },
            new RolePermission { RoleId = 1, PermissionId = 15 },
            new RolePermission { RoleId = 1, PermissionId = 16 },
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 9 },
            new RolePermission { RoleId = 2, PermissionId = 11 },
            new RolePermission { RoleId = 2, PermissionId = 13 },
            new RolePermission { RoleId = 2, PermissionId = 15 },
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 2 },
            new RolePermission { RoleId = 3, PermissionId = 3 },
            new RolePermission { RoleId = 3, PermissionId = 4 },
            new RolePermission { RoleId = 3, PermissionId = 5 },
            new RolePermission { RoleId = 3, PermissionId = 6 },
            new RolePermission { RoleId = 3, PermissionId = 7 },
            new RolePermission { RoleId = 3, PermissionId = 8 },
            new RolePermission { RoleId = 3, PermissionId = 9 },
            new RolePermission { RoleId = 3, PermissionId = 10 },
            new RolePermission { RoleId = 3, PermissionId = 11 },
            new RolePermission { RoleId = 3, PermissionId = 12 },
            new RolePermission { RoleId = 3, PermissionId = 13 },
            new RolePermission { RoleId = 3, PermissionId = 14 },
            new RolePermission { RoleId = 3, PermissionId = 15 },
            new RolePermission { RoleId = 3, PermissionId = 16 },
            new RolePermission { RoleId = 3, PermissionId = 17 },
            new RolePermission { RoleId = 3, PermissionId = 18 },
            new RolePermission { RoleId = 3, PermissionId = 19 },
            new RolePermission { RoleId = 3, PermissionId = 20 },
            new RolePermission { RoleId = 4, PermissionId = 21 });
    }
}
