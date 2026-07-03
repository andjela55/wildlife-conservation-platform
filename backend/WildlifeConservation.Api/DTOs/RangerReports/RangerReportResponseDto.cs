using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.RangerReports;

public record RangerReportResponseDto(
    int Id,
    int? AnimalId,
    int UserId,
    ReportType ReportType,
    Severity Severity,
    decimal Latitude,
    decimal Longitude,
    string Description,
    DateTime CreatedAt);
