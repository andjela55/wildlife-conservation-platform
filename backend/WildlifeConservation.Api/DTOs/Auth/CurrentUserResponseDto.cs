using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.Auth;

public record CurrentUserResponseDto(
    int Id,
    string FullName,
    string Email,
    UserRole Role,
    string? AssignedLocationName,
    decimal? AssignedLatitude,
    decimal? AssignedLongitude,
    int? AssignedMapZoom);
