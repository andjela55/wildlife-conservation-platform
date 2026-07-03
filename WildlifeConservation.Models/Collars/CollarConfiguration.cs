using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Collars;

public class CollarConfiguration : IEntityTypeConfiguration<Collar>
{
    public void Configure(EntityTypeBuilder<Collar> builder)
    {
        builder.Property(x => x.SerialNumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(120);
        builder.Property(x => x.Manufacturer).HasMaxLength(120);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => x.SerialNumber).IsUnique();

        builder.HasData(
            new Collar { Id = 1, SerialNumber = "WC-GPS-1001", Model = "SavannaTrack X1", Manufacturer = "EcoTelemetry", Status = CollarStatus.Assigned, Notes = "Solar-assisted battery pack." },
            new Collar { Id = 2, SerialNumber = "WC-GPS-1002", Model = "SavannaTrack X1", Manufacturer = "EcoTelemetry", Status = CollarStatus.Assigned, Notes = "Configured for elephant migration interval." },
            new Collar { Id = 3, SerialNumber = "WC-GPS-2001", Model = "PredatorLink P4", Manufacturer = "RangeSense", Status = CollarStatus.Assigned, Notes = "High-frequency predator tracking." },
            new Collar { Id = 4, SerialNumber = "WC-GPS-3001", Model = "CanidTrail C2", Manufacturer = "RangeSense", Status = CollarStatus.Assigned, Notes = "Cold-weather battery profile." },
            new Collar { Id = 5, SerialNumber = "WC-GPS-1003", Model = "ForestTrack F2", Manufacturer = "EcoTelemetry", Status = CollarStatus.Assigned, Notes = "Dense canopy satellite profile." });
    }
}
