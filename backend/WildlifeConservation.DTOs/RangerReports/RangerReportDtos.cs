using System.ComponentModel.DataAnnotations;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.DTOs;

public record CreateRangerReportDto
{
    public int? AnimalId { get; init; }

    public ReportType ReportType { get; init; } = ReportType.Other;
    public Severity Severity { get; init; } = Severity.Low;

    [Range(-90, 90)]
    public decimal Latitude { get; init; }

    [Range(-180, 180)]
    public decimal Longitude { get; init; }

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
