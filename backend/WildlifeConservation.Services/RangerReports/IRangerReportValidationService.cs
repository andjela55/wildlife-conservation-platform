namespace WildlifeConservation.Services.RangerReports;

public interface IRangerReportValidationService
{
    Task<RangerReport> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task ValidateCreateAsync(CreateRangerReportDto dto, int userId, CancellationToken cancellationToken);
    Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken);
}
