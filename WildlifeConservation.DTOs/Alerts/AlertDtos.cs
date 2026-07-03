using System.ComponentModel.DataAnnotations;
using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.DTOs;

public record CreateAlertDto
{
    [Range(1, int.MaxValue)]
    public int AnimalId { get; init; }

    public int? CollarId { get; init; }
    public int? CreatedByUserId { get; init; }
    public AlertType AlertType { get; init; } = AlertType.Other;
    public Severity Severity { get; init; } = Severity.Low;

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record ResolveAlertDto
{
    public DateTime? ResolvedAt { get; init; }
}
