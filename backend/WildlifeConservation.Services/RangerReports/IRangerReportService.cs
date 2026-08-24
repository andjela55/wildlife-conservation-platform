namespace WildlifeConservation.Services.RangerReports;

public interface IRangerReportService
{
    Task<PagedResult<RangerReport>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<RangerReport> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RangerReport> CreateAsync(CreateRangerReportDto dto, int userId, CancellationToken cancellationToken = default);
    Task<PagedResult<RangerReport>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default);
}
