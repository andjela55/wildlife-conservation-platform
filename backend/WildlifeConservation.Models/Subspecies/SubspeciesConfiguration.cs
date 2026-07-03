using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Subspecies;

public class SubspeciesConfiguration : IEntityTypeConfiguration<Subspecies>
{
    public void Configure(EntityTypeBuilder<Subspecies> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.HasOne(x => x.Species)
            .WithMany(x => x.Subspecies)
            .HasForeignKey(x => x.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.SpeciesId, x.Name }).IsUnique();

        builder.HasData(
            new Subspecies { Id = 1, SpeciesId = 1, Name = "African Savanna Elephant", Description = "Open savanna elephant population." },
            new Subspecies { Id = 2, SpeciesId = 1, Name = "Asian Elephant", Description = "Forest and grassland elephant population." },
            new Subspecies { Id = 3, SpeciesId = 2, Name = "East African Lion", Description = "Lion population tracked across protected ranges." },
            new Subspecies { Id = 4, SpeciesId = 3, Name = "Eurasian Wolf", Description = "Wolf population monitored near reserve corridors." });
    }
}
