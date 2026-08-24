namespace WildlifeConservation.Services.Collars;

public class CollarService(
    ICollarRepository collarRepository,
    ICollarValidationService validationService,
    IMapper mapper,
    IAlertRepository alertRepository,
    ILocationPointRepository locationPointRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : ICollarService
{
    public async Task<PagedResult<Collar>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await collarRepository.Query()
            .OrderBy(x => x.SerialNumber)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Collar> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Collar> CreateAsync(UpsertCollarDto dto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateCreateAsync(dto, cancellationToken);

        var collar = mapper.Map<Collar>(dto);

        collar = await collarRepository.InsertAsync(collar, cancellationToken);

        return collar;
    }

    public async Task<Collar> UpdateAsync(int id, UpsertCollarDto dto, CancellationToken cancellationToken = default)
    {
        var collar = await validationService.ValidateUpdateAsync(id, dto, cancellationToken);

        mapper.Map(dto, collar);

        collar = await collarRepository.UpdateAsync(collar, cancellationToken);

        return collar;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var collar = await validationService.GetRequiredAsync(id, cancellationToken);
        await using var transaction = await collarRepository.StartTransactionAsync(cancellationToken);
        try
        {
            var alerts = await alertRepository.Query().Where(x => x.CollarId == id).ToListAsync(cancellationToken);
            var locations = await locationPointRepository.Query().Where(x => x.CollarId == id).ToListAsync(cancellationToken);
            var assignments = await collarAssignmentRepository.Query().Where(x => x.CollarId == id).ToListAsync(cancellationToken);
            await alertRepository.DeleteRangeAsync(alerts, cancellationToken);
            await locationPointRepository.DeleteRangeAsync(locations, cancellationToken);
            await collarAssignmentRepository.DeleteRangeAsync(assignments, cancellationToken);
            await collarRepository.DeleteAsync(collar, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
