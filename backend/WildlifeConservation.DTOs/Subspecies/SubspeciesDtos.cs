using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public record UpsertSubspeciesDto
{
    [Range(1, int.MaxValue)]
    public int SpeciesId { get; init; }

    [Required]
    [StringLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;
}
