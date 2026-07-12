using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public record LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;
}
