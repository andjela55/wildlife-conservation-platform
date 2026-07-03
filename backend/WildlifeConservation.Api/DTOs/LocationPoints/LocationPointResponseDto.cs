using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.LocationPoints;

public record LocationPointResponseDto(
    int Id,
    int AnimalId,
    int CollarId,
    decimal Latitude,
    decimal Longitude,
    decimal? Altitude,
    DateTime RecordedAt,
    SignalType SignalType,
    string? Notes);
