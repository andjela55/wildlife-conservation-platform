namespace WildlifeConservation.Services.Collars;

public class CollarValidationService(
    ICollarRepository collarRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : ICollarValidationService
{
    public async Task<Collar> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await collarRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar with id {id} was not found.");

    public async Task ValidateCreateAsync(UpsertCollarDto dto, CancellationToken cancellationToken)
    {
        var serialNumber = ServiceHelpers.RequiredText(dto.SerialNumber, nameof(dto.SerialNumber));
        await EnsureSerialNumberIsUniqueAsync(serialNumber, existingId: null, cancellationToken);
    }

    public async Task<Collar> ValidateUpdateAsync(int id, UpsertCollarDto dto, CancellationToken cancellationToken)
    {
        var collar = await GetRequiredAsync(id, cancellationToken);
        var serialNumber = ServiceHelpers.RequiredText(dto.SerialNumber, nameof(dto.SerialNumber));
        await EnsureSerialNumberIsUniqueAsync(serialNumber, id, cancellationToken);
        var hasActiveAssignment = await collarAssignmentRepository.Query()
            .AnyAsync(x => x.CollarId == id && x.UnassignedAt == null, cancellationToken);
        if (hasActiveAssignment && dto.Status != collar.Status && dto.Status != CollarStatus.Assigned)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "An actively assigned collar must keep the Assigned status until it is unassigned.");
        }

        return collar;
    }

    private async Task EnsureSerialNumberIsUniqueAsync(string serialNumber, int? existingId, CancellationToken cancellationToken)
    {
        var duplicate = await collarRepository.Query().AnyAsync(
            x => x.SerialNumber.ToLower() == serialNumber.ToLower() && (!existingId.HasValue || x.Id != existingId.Value),
            cancellationToken);
        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Collar serial number '{serialNumber}' already exists.");
        }
    }
}
