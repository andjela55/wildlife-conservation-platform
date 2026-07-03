using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeConservation.Models.SeedData;

namespace WildlifeConservation.Models.Alerts;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.HasOne(x => x.Animal)
            .WithMany(x => x.Alerts)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Collar)
            .WithMany(x => x.Alerts)
            .HasForeignKey(x => x.CollarId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.Alerts)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Alert { Id = 1, AnimalId = 1, CollarId = 1, CreatedByUserId = null, AlertType = AlertType.LeftSafeZone, Severity = Severity.High, Description = "Animal moved close to the southern reserve boundary.", IsResolved = false, CreatedAt = SeedTimes.T3 },
            new Alert { Id = 2, AnimalId = 3, CollarId = 3, CreatedByUserId = null, AlertType = AlertType.CollarBatteryLow, Severity = Severity.Medium, Description = "Collar battery is below threshold.", IsResolved = false, CreatedAt = SeedTimes.T4 },
            new Alert { Id = 3, AnimalId = 4, CollarId = 4, CreatedByUserId = 1, AlertType = AlertType.Manual, Severity = Severity.Medium, Description = "Manual watch requested after boundary movement.", IsResolved = true, CreatedAt = SeedTimes.T2, ResolvedAt = SeedTimes.T4 },
            new Alert { Id = 4, AnimalId = 5, CollarId = null, CreatedByUserId = 2, AlertType = AlertType.Other, Severity = Severity.Low, Description = "Review relocation zone condition after ranger report.", IsResolved = false, CreatedAt = SeedTimes.T4 });
    }
}
