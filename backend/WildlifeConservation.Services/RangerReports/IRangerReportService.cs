namespace WildlifeConservation.Services.RangerReports;

public interface IRangerReportService
{
    Task<List<RangerReport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RangerReport> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RangerReport> CreateAsync(CreateRangerReportDto dto, CancellationToken cancellationToken = default);
}
