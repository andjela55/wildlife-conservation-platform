using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WildlifeConservation.Models.Animals;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Subspecies)
            .WithMany(x => x.Animals)
            .HasForeignKey(x => x.SubspeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Animal { Id = 1, Name = "Amara", SubspeciesId = 1, Sex = AnimalSex.Female, DateOfBirth = new DateTime(2014, 5, 12, 0, 0, 0, DateTimeKind.Utc), Notes = "Matriarch of a small herd.", IsActive = true },
            new Animal { Id = 2, Name = "Kito", SubspeciesId = 1, Sex = AnimalSex.Male, DateOfBirth = new DateTime(2017, 9, 3, 0, 0, 0, DateTimeKind.Utc), Notes = "Often seen near the western watering route.", IsActive = true },
            new Animal { Id = 3, Name = "Sera", SubspeciesId = 3, Sex = AnimalSex.Female, DateOfBirth = new DateTime(2019, 2, 18, 0, 0, 0, DateTimeKind.Utc), Notes = "Adult lioness in northern pride.", IsActive = true },
            new Animal { Id = 4, Name = "Batu", SubspeciesId = 4, Sex = AnimalSex.Male, DateOfBirth = null, Notes = "Dispersing male observed near reserve edge.", IsActive = true },
            new Animal { Id = 5, Name = "Nila", SubspeciesId = 2, Sex = AnimalSex.Female, DateOfBirth = new DateTime(2016, 7, 22, 0, 0, 0, DateTimeKind.Utc), Notes = "Relocated after crop-raiding incident.", IsActive = true });
    }
}
