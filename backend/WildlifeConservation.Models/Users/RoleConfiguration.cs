using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Users;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(240).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasData(
            new Role { Id = 1, Name = "Ranger", Description = "Field staff who record and maintain conservation data." },
            new Role { Id = 2, Name = "Researcher", Description = "Read-only access to conservation data." },
            new Role { Id = 3, Name = "Admin", Description = "Platform administration and conservation data management." },
            new Role { Id = 4, Name = "Master", Description = "Unrestricted platform administration." });
    }
}
