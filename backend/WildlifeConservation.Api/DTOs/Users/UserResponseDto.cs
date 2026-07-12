using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.Users;

public record UserResponseDto(
    int Id,
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive,
    string? AssignedLocationName,
    decimal? AssignedLatitude,
    decimal? AssignedLongitude,
    int? AssignedMapZoom);
