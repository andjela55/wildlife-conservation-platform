using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public record UpdateUserAssignedAreaDto
{
    [StringLength(160)]
    public string? AssignedLocationName { get; init; }

    [Range(-90, 90)]
    public decimal? AssignedLatitude { get; init; }

    [Range(-180, 180)]
    public decimal? AssignedLongitude { get; init; }

    [Range(1, 18)]
    public int? AssignedMapZoom { get; init; }
}
