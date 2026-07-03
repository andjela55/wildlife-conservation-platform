namespace WildlifeConservation.Api.DTOs.CollarAssignments;

public record CollarAssignmentResponseDto(
    int Id,
    int AnimalId,
    int CollarId,
    DateTime AssignedAt,
    DateTime? UnassignedAt,
    string? Reason,
    string? Notes);
