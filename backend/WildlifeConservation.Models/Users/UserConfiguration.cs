using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Models.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FullName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(x => x.PasswordSalt).HasMaxLength(64).IsRequired();
        builder.Property(x => x.AssignedLocationName).HasMaxLength(160);
        builder.Property(x => x.AssignedLatitude).HasPrecision(9, 6);
        builder.Property(x => x.AssignedLongitude).HasPrecision(9, 6);
        builder.HasIndex(x => x.Email).IsUnique();

        const string adminSalt = "QWRtaW5TZWVkU2FsdDEyMw==";
        const string masterSalt = "TWFzdGVyU2VlZFNhbHQxMjM=";
        const string rangerSalt = "UmFuZ2VyU2VlZFNhbHQxMjM=";
        const string researcherSalt = "UmVzZWFyY2hTZWVkU2FsdDE=";

        builder.HasData(
            new User
            {
                Id = 1,
                FullName = "Maya Nkosi",
                Email = "maya.ranger@example.org",
                PasswordSalt = rangerSalt,
                PasswordHash = PasswordHasher.HashPassword("Ranger123!", rangerSalt),
                IsActive = true
            },
            new User
            {
                Id = 2,
                FullName = "Luka Petrovic",
                Email = "luka.research@example.org",
                PasswordSalt = researcherSalt,
                PasswordHash = PasswordHasher.HashPassword("Researcher123!", researcherSalt),
                IsActive = true
            },
            new User
            {
                Id = 3,
                FullName = "Anika Rao",
                Email = "anika.admin@example.org",
                PasswordSalt = adminSalt,
                PasswordHash = PasswordHasher.HashPassword("Admin123!", adminSalt),
                IsActive = true
            },
            new User
            {
                Id = 4,
                FullName = "Elena Markovic",
                Email = "elena.master@example.org",
                PasswordSalt = masterSalt,
                PasswordHash = PasswordHasher.HashPassword("Master123!", masterSalt),
                IsActive = true
            });
    }
}
