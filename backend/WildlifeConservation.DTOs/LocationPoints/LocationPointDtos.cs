using System.ComponentModel.DataAnnotations;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.DTOs;

public record CreateLocationPointDto
{
    [Range(1, int.MaxValue)]
    public int AnimalId { get; init; }

    [Range(1, int.MaxValue)]
    public int CollarId { get; init; }

    [Range(-90, 90)]
    public decimal Latitude { get; init; }

    [Range(-180, 180)]
    public decimal Longitude { get; init; }

    public decimal? Altitude { get; init; }
    public DateTime RecordedAt { get; init; } = DateTime.UtcNow;
    public SignalType SignalType { get; init; } = SignalType.Simulator;

    [StringLength(1000)]
    public string? Notes { get; init; }
}
