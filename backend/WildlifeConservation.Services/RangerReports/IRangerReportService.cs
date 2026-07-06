namespace WildlifeConservation.Services.RangerReports;

public interface IRangerReportService
{
    Task<PagedResult<RangerReport>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<RangerReport> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RangerReport> CreateAsync(CreateRangerReportDto dto, CancellationToken cancellationToken = default);
}
