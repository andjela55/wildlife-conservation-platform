using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeConservation.Models.SeedData;

namespace WildlifeConservation.Models.RangerReports;

public class RangerReportConfiguration : IEntityTypeConfiguration<RangerReport>
{
    public void Configure(EntityTypeBuilder<RangerReport> builder)
    {
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.HasOne(x => x.Animal)
            .WithMany(x => x.RangerReports)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User)
            .WithMany(x => x.RangerReports)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new RangerReport { Id = 1, AnimalId = 1, UserId = 1, ReportType = ReportType.Sighting, Severity = Severity.Low, Latitude = -1.935000m, Longitude = 35.153000m, Description = "Herd sighted moving calmly toward grazing area.", CreatedAt = SeedTimes.T2 },
            new RangerReport { Id = 2, AnimalId = 3, UserId = 1, ReportType = ReportType.CollarIssue, Severity = Severity.Medium, Latitude = -1.708500m, Longitude = 35.018000m, Description = "Collar antenna appears tilted but still reporting.", CreatedAt = SeedTimes.T4 },
            new RangerReport { Id = 3, AnimalId = null, UserId = 2, ReportType = ReportType.PoachingSigns, Severity = Severity.High, Latitude = -1.774000m, Longitude = 35.097000m, Description = "Fresh snares found along dry stream bed.", CreatedAt = SeedTimes.T3 },
            new RangerReport { Id = 4, AnimalId = 4, UserId = 1, ReportType = ReportType.Sighting, Severity = Severity.Low, Latitude = 44.142800m, Longitude = 20.462300m, Description = "Tracks and visual confirmation near corridor camera.", CreatedAt = SeedTimes.T4 });
    }
}
