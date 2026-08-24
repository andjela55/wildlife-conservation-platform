namespace WildlifeConservation.Api.DTOs.Users;

public record RoleResponseDto(
    int Id,
    string Name,
    string Description,
    IReadOnlyCollection<PermissionCode> Permissions);
