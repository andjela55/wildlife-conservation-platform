namespace WildlifeConservation.Api.DTOs.Auth;

public record CurrentUserResponseDto(
    int Id,
    string FullName,
    string Email,
    IReadOnlyCollection<RoleResponseDto> Roles,
    IReadOnlyCollection<PermissionCode> Permissions,
    string? AssignedLocationName,
    decimal? AssignedLatitude,
    decimal? AssignedLongitude,
    int? AssignedMapZoom);
