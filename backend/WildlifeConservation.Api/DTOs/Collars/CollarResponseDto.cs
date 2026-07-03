using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.Collars;

public record CollarResponseDto(
    int Id,
    string SerialNumber,
    string? Model,
    string? Manufacturer,
    CollarStatus Status,
    string? Notes);
