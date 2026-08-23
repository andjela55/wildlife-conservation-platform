namespace WildlifeConservation.Services.Alerts;

public interface IAlertValidationService
{
    Task<Alert> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task<int?> ValidateCreateAsync(
        CreateAlertDto dto,
        int createdByUserId,
        DateTime createdAt,
        CancellationToken cancellationToken);
    Task<(Alert Alert, DateTime ResolvedAt)> ValidateResolveAsync(
        int id,
        ResolveAlertDto dto,
        CancellationToken cancellationToken);
    Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken);
}
