namespace WildlifeConservation.Api.DTOs.Auth;

public record LoginResponseDto(
    string Token,
    DateTime ExpiresAt);
