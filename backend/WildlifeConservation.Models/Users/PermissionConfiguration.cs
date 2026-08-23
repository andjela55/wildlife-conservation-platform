using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Users;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(240).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasData(
            new Permission { Id = 1, Code = PermissionCode.AnimalsRead, Description = "View animals." },
            new Permission { Id = 2, Code = PermissionCode.AnimalsWrite, Description = "Create and update animals." },
            new Permission { Id = 3, Code = PermissionCode.AlertsRead, Description = "View alerts." },
            new Permission { Id = 4, Code = PermissionCode.AlertsWrite, Description = "Create and update alerts." },
            new Permission { Id = 5, Code = PermissionCode.CollarsRead, Description = "View collars." },
            new Permission { Id = 6, Code = PermissionCode.CollarsWrite, Description = "Create and update collars." },
            new Permission { Id = 7, Code = PermissionCode.CollarAssignmentsRead, Description = "View collar assignments." },
            new Permission { Id = 8, Code = PermissionCode.CollarAssignmentsWrite, Description = "Create and update collar assignments." },
            new Permission { Id = 9, Code = PermissionCode.LocationPointsRead, Description = "View location points." },
            new Permission { Id = 10, Code = PermissionCode.LocationPointsWrite, Description = "Create and update location points." },
            new Permission { Id = 11, Code = PermissionCode.RangerReportsRead, Description = "View ranger reports." },
            new Permission { Id = 12, Code = PermissionCode.RangerReportsWrite, Description = "Create and update ranger reports." },
            new Permission { Id = 13, Code = PermissionCode.SpeciesRead, Description = "View species." },
            new Permission { Id = 14, Code = PermissionCode.SpeciesWrite, Description = "Create and update species." },
            new Permission { Id = 15, Code = PermissionCode.SubspeciesRead, Description = "View subspecies." },
            new Permission { Id = 16, Code = PermissionCode.SubspeciesWrite, Description = "Create and update subspecies." },
            new Permission { Id = 17, Code = PermissionCode.UsersRead, Description = "View users." },
            new Permission { Id = 18, Code = PermissionCode.UsersWrite, Description = "Create and update users." },
            new Permission { Id = 19, Code = PermissionCode.RolesRead, Description = "View roles." },
            new Permission { Id = 20, Code = PermissionCode.RolesWrite, Description = "Create and update roles." },
            new Permission { Id = 21, Code = PermissionCode.Master, Description = "Unrestricted platform access." });
    }
}
