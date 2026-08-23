namespace WildlifeConservation.Services.Collars;

public class CollarService(
    ICollarRepository collarRepository,
    ICollarAssignmentRepository collarAssignmentRepository,
    IMapper mapper) : ICollarService
{
    public async Task<PagedResult<Collar>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await collarRepository.Query()
            .OrderBy(x => x.SerialNumber)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Collar> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var collar = await collarRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return collar is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar with id {id} was not found.")
            : collar;
    }

    public async Task<Collar> CreateAsync(UpsertCollarDto dto, CancellationToken cancellationToken = default)
    {
        var serialNumber = ServiceHelpers.RequiredText(dto.SerialNumber, nameof(dto.SerialNumber));
        await EnsureSerialNumberIsUniqueAsync(serialNumber, null, cancellationToken);

        var collar = mapper.Map<Collar>(dto);

        collar = await collarRepository.InsertAsync(collar, cancellationToken);

        return collar;
    }

    public async Task<Collar> UpdateAsync(int id, UpsertCollarDto dto, CancellationToken cancellationToken = default)
    {
        var collar = await collarRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Collar with id {id} was not found.");

        var serialNumber = ServiceHelpers.RequiredText(dto.SerialNumber, nameof(dto.SerialNumber));
        await EnsureSerialNumberIsUniqueAsync(serialNumber, id, cancellationToken);

        var hasActiveAssignment = await collarAssignmentRepository.Query()
            .AnyAsync(x => x.CollarId == id && x.UnassignedAt == null, cancellationToken);

        if (hasActiveAssignment && dto.Status != collar.Status && dto.Status != CollarStatus.Assigned)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, "An actively assigned collar must keep the Assigned status until it is unassigned.");
        }

        mapper.Map(dto, collar);

        collar = await collarRepository.UpdateAsync(collar, cancellationToken);

        return collar;
    }

    private async Task EnsureSerialNumberIsUniqueAsync(string serialNumber, int? existingId, CancellationToken cancellationToken)
    {
        var duplicate = await collarRepository.Query()
            .AnyAsync(x => x.SerialNumber.ToLower() == serialNumber.ToLower() && (!existingId.HasValue || x.Id != existingId.Value), cancellationToken);

        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Collar serial number '{serialNumber}' already exists.");
        }
    }
}
