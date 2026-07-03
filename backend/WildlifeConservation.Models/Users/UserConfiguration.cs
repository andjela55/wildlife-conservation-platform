using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FullName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasData(
            new User { Id = 1, FullName = "Maya Nkosi", Email = "maya.ranger@example.org", Role = UserRole.Ranger, IsActive = true },
            new User { Id = 2, FullName = "Luka Petrovic", Email = "luka.research@example.org", Role = UserRole.Researcher, IsActive = true },
            new User { Id = 3, FullName = "Anika Rao", Email = "anika.admin@example.org", Role = UserRole.Admin, IsActive = true });
    }
}
