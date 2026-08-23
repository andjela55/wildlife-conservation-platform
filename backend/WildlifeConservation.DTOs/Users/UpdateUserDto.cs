using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public record UpdateUserDto
{
    [Required]
    [StringLength(160)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [StringLength(100, MinimumLength = 8)]
    public string? Password { get; init; }

    [MinLength(1)]
    public IReadOnlyCollection<int> RoleIds { get; init; } = Array.Empty<int>();
    public bool IsActive { get; init; }

    [StringLength(160)]
    public string? AssignedLocationName { get; init; }

    [Range(-90, 90)]
    public decimal? AssignedLatitude { get; init; }

    [Range(-180, 180)]
    public decimal? AssignedLongitude { get; init; }

    [Range(1, 18)]
    public int? AssignedMapZoom { get; init; }
}
