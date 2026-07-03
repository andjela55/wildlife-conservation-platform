using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public record CreateCollarAssignmentDto
{
    [Range(1, int.MaxValue)]
    public int AnimalId { get; init; }

    [Range(1, int.MaxValue)]
    public int CollarId { get; init; }

    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    [StringLength(250)]
    public string? Reason { get; init; }

    [StringLength(1000)]
    public string? Notes { get; init; }
}

public record UnassignCollarDto
{
    public DateTime? UnassignedAt { get; init; }

    [StringLength(250)]
    public string? Reason { get; init; }

    [StringLength(1000)]
    public string? Notes { get; init; }
}
