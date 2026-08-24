using System.ComponentModel.DataAnnotations;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.DTOs;

public record UpsertAnimalDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SubspeciesId { get; init; }

    public AnimalSex Sex { get; init; } = AnimalSex.Unknown;
    public DateTime? DateOfBirth { get; init; }

    [StringLength(1000)]
    public string? Notes { get; init; }

    public bool IsActive { get; init; } = true;
}
