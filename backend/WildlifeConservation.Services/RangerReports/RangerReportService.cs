namespace WildlifeConservation.Services.RangerReports;

public class RangerReportService(
    IRangerReportRepository rangerReportRepository,
    IRangerReportValidationService validationService,
    IMapper mapper) : IRangerReportService
{
    public async Task<PagedResult<RangerReport>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await rangerReportRepository.Query()
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<RangerReport> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<RangerReport> CreateAsync(CreateRangerReportDto dto, int userId, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateCreateAsync(dto, userId, cancellationToken);

        var report = mapper.Map<RangerReport>(dto);
        report.UserId = userId;
        report.CreatedAt = DateTime.UtcNow;

        report = await rangerReportRepository.InsertAsync(report, cancellationToken);

        return report;
    }

    public async Task<PagedResult<RangerReport>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateAnimalAsync(animalId, cancellationToken);

        return await rangerReportRepository.Query()
            .Where(x => x.AnimalId == animalId)
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
