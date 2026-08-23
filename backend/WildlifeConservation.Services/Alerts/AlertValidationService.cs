namespace WildlifeConservation.Services.Alerts;

public class AlertValidationService(
    IAlertRepository alertRepository,
    IAnimalRepository animalRepository,
    ICollarAssignmentRepository collarAssignmentRepository,
    IUserRepository userRepository) : IAlertValidationService
{
    public async Task<Alert> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await alertRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Alert with id {id} was not found.");

    public async Task<int?> ValidateCreateAsync(
        CreateAlertDto dto,
        int createdByUserId,
        DateTime createdAt,
        CancellationToken cancellationToken)
    {
        await ValidateAnimalAsync(dto.AnimalId, cancellationToken);
        await ServiceHelpers.EnsureFoundAsync(
            userRepository.GetByIdAsync(createdByUserId, cancellationToken), createdByUserId, "User");
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));

        var activeAssignment = await collarAssignmentRepository.Query()
            .Where(x =>
                x.AnimalId == dto.AnimalId &&
                x.AssignedAt <= createdAt &&
                (x.UnassignedAt == null || x.UnassignedAt >= createdAt))
            .OrderByDescending(x => x.AssignedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return activeAssignment?.CollarId;
    }

    public async Task<(Alert Alert, DateTime ResolvedAt)> ValidateResolveAsync(
        int id,
        ResolveAlertDto dto,
        CancellationToken cancellationToken)
    {
        var alert = await GetRequiredAsync(id, cancellationToken);
        if (alert.IsResolved)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Alert is already resolved.");
        }

        var resolvedAt = ServiceHelpers.AsUtc(dto.ResolvedAt ?? DateTime.UtcNow);
        if (resolvedAt < alert.CreatedAt)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "ResolvedAt cannot be earlier than CreatedAt.");
        }

        return (alert, resolvedAt);
    }

    public async Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken)
    {
        await ServiceHelpers.EnsureFoundAsync(
            animalRepository.GetByIdAsync(animalId, cancellationToken), animalId, "Animal");
    }
}
