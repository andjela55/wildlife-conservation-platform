using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Api.DTOs.Animals;

public record AnimalResponseDto(
    int Id,
    string Name,
    int SubspeciesId,
    AnimalSex Sex,
    DateTime? DateOfBirth,
    string? Notes,
    bool IsActive);
