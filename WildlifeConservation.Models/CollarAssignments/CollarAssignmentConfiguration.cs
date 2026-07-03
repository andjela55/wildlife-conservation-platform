using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeConservation.Models.SeedData;

namespace WildlifeConservation.Models.CollarAssignments;

public class CollarAssignmentConfiguration : IEntityTypeConfiguration<CollarAssignment>
{
    public void Configure(EntityTypeBuilder<CollarAssignment> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(250);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Animal)
            .WithMany(x => x.CollarAssignments)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Collar)
            .WithMany(x => x.CollarAssignments)
            .HasForeignKey(x => x.CollarId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.AnimalId).IsUnique().HasFilter("\"UnassignedAt\" IS NULL");
        builder.HasIndex(x => x.CollarId).IsUnique().HasFilter("\"UnassignedAt\" IS NULL");

        builder.HasData(
            new CollarAssignment { Id = 1, AnimalId = 1, CollarId = 1, AssignedAt = SeedTimes.T1, Notes = "Initial collaring during health check." },
            new CollarAssignment { Id = 2, AnimalId = 2, CollarId = 2, AssignedAt = SeedTimes.T1, Notes = "Assigned after herd survey." },
            new CollarAssignment { Id = 3, AnimalId = 3, CollarId = 3, AssignedAt = SeedTimes.T1, Notes = "Assigned near northern pride range." },
            new CollarAssignment { Id = 4, AnimalId = 4, CollarId = 4, AssignedAt = SeedTimes.T1, Notes = "Assigned near reserve boundary." },
            new CollarAssignment { Id = 5, AnimalId = 5, CollarId = 5, AssignedAt = SeedTimes.T1, Notes = "Assigned after relocation release." });
    }
}
