using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeConservation.Models.SeedData;

namespace WildlifeConservation.Models.LocationPoints;

public class LocationPointConfiguration : IEntityTypeConfiguration<LocationPoint>
{
    public void Configure(EntityTypeBuilder<LocationPoint> builder)
    {
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.Property(x => x.Altitude).HasPrecision(8, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Animal)
            .WithMany(x => x.LocationPoints)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Collar)
            .WithMany(x => x.LocationPoints)
            .HasForeignKey(x => x.CollarId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.AnimalId, x.RecordedAt });
        builder.HasIndex(x => new { x.CollarId, x.RecordedAt });

        builder.HasData(
            new LocationPoint { Id = 1, AnimalId = 1, CollarId = 1, Latitude = -1.942345m, Longitude = 35.148900m, Altitude = 1524.50m, RecordedAt = SeedTimes.T1, SignalType = SignalType.Satellite, Notes = "Near southern water point." },
            new LocationPoint { Id = 2, AnimalId = 1, CollarId = 1, Latitude = -1.935120m, Longitude = 35.153420m, Altitude = 1527.10m, RecordedAt = SeedTimes.T2, SignalType = SignalType.Satellite },
            new LocationPoint { Id = 3, AnimalId = 2, CollarId = 2, Latitude = -1.851100m, Longitude = 35.221200m, Altitude = 1495.00m, RecordedAt = SeedTimes.T2, SignalType = SignalType.Cellular },
            new LocationPoint { Id = 4, AnimalId = 2, CollarId = 2, Latitude = -1.844980m, Longitude = 35.229310m, Altitude = 1492.20m, RecordedAt = SeedTimes.T3, SignalType = SignalType.Cellular, Notes = "Moving east." },
            new LocationPoint { Id = 5, AnimalId = 3, CollarId = 3, Latitude = -1.712420m, Longitude = 35.012780m, Altitude = 1411.00m, RecordedAt = SeedTimes.T2, SignalType = SignalType.Satellite, Notes = "Northern pride range." },
            new LocationPoint { Id = 6, AnimalId = 3, CollarId = 3, Latitude = -1.708700m, Longitude = 35.018050m, Altitude = 1409.30m, RecordedAt = SeedTimes.T4, SignalType = SignalType.Satellite },
            new LocationPoint { Id = 7, AnimalId = 4, CollarId = 4, Latitude = 44.138700m, Longitude = 20.458100m, Altitude = 864.00m, RecordedAt = SeedTimes.T2, SignalType = SignalType.LoRaWAN, Notes = "Boundary corridor." },
            new LocationPoint { Id = 8, AnimalId = 4, CollarId = 4, Latitude = 44.142900m, Longitude = 20.462450m, Altitude = 872.30m, RecordedAt = SeedTimes.T4, SignalType = SignalType.LoRaWAN },
            new LocationPoint { Id = 9, AnimalId = 5, CollarId = 5, Latitude = 11.551200m, Longitude = 76.231500m, Altitude = 912.00m, RecordedAt = SeedTimes.T2, SignalType = SignalType.Satellite, Notes = "Post-release forest range." },
            new LocationPoint { Id = 10, AnimalId = 5, CollarId = 5, Latitude = 11.557400m, Longitude = 76.236100m, Altitude = 918.80m, RecordedAt = SeedTimes.T4, SignalType = SignalType.Satellite });
    }
}
