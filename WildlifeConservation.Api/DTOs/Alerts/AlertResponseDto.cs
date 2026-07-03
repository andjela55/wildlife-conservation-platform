using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.Alerts;

public record AlertResponseDto(
    int Id,
    int AnimalId,
    int? CollarId,
    int? CreatedByUserId,
    AlertType AlertType,
    Severity Severity,
    string Description,
    bool IsResolved,
    DateTime CreatedAt,
    DateTime? ResolvedAt);
