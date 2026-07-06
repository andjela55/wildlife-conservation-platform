using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.LocationPoints;

public record LocationPointReceivedDto(
    int Id,
    int AnimalId,
    string AnimalName,
    int CollarId,
    string CollarSerialNumber,
    decimal Latitude,
    decimal Longitude,
    decimal? Altitude,
    DateTime RecordedAt,
    SignalType SignalType,
    string? Notes);
