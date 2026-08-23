namespace WildlifeConservation.Services.RangerReports;

public class RangerReportValidationService(
    IRangerReportRepository rangerReportRepository,
    IAnimalRepository animalRepository,
    IUserRepository userRepository) : IRangerReportValidationService
{
    public async Task<RangerReport> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await rangerReportRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Ranger report with id {id} was not found.");

    public async Task ValidateCreateAsync(CreateRangerReportDto dto, int userId, CancellationToken cancellationToken)
    {
        if (dto.AnimalId.HasValue)
        {
            await ValidateAnimalAsync(dto.AnimalId.Value, cancellationToken);
        }

        await ServiceHelpers.EnsureFoundAsync(
            userRepository.GetByIdAsync(userId, cancellationToken), userId, "User");
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
    }

    public async Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken)
    {
        await ServiceHelpers.EnsureFoundAsync(
            animalRepository.GetByIdAsync(animalId, cancellationToken), animalId, "Animal");
    }
}
