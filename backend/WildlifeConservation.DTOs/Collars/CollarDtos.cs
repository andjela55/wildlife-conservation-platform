using System.ComponentModel.DataAnnotations;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.DTOs;

public record UpsertCollarDto
{
    [Required]
    [StringLength(80)]
    public string SerialNumber { get; init; } = string.Empty;

    [StringLength(120)]
    public string? Model { get; init; }

    [StringLength(120)]
    public string? Manufacturer { get; init; }

    public CollarStatus Status { get; init; } = CollarStatus.Available;

    [StringLength(1000)]
    public string? Notes { get; init; }
}
