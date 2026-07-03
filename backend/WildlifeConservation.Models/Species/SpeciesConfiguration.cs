using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Species;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasData(
            new Species { Id = 1, Name = "Elephant", Description = "Large herbivorous mammals monitored for migration and habitat pressure." },
            new Species { Id = 2, Name = "Lion", Description = "Large carnivores monitored for pride movement and human-wildlife conflict." },
            new Species { Id = 3, Name = "Wolf", Description = "Social carnivores monitored for range behavior and conservation planning." });
    }
}
