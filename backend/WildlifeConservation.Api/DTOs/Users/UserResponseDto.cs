namespace WildlifeConservation.Api.DTOs.Users;

public record UserResponseDto(
    int Id,
    string FullName,
    string Email,
    IReadOnlyCollection<RoleResponseDto> Roles,
    bool IsActive,
    string? AssignedLocationName,
    decimal? AssignedLatitude,
    decimal? AssignedLongitude,
    int? AssignedMapZoom);
