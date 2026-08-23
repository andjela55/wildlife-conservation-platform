namespace WildlifeConservation.Services.RangerReports;

public class RangerReportService(
    IRangerReportRepository rangerReportRepository,
    IAnimalRepository animalRepository,
    IUserRepository userRepository,
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
        var report = await rangerReportRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return report is null
            ? throw new ServiceException((int)HttpStatusCode.NotFound, $"Ranger report with id {id} was not found.")
            : report;
    }

    public async Task<RangerReport> CreateAsync(CreateRangerReportDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (dto.AnimalId.HasValue)
        {
            await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId.Value, cancellationToken), dto.AnimalId.Value, "Animal");
        }

        await ServiceHelpers.EnsureFoundAsync(userRepository.GetByIdAsync(userId, cancellationToken), userId, "User");

        var report = mapper.Map<RangerReport>(dto);
        report.UserId = userId;
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        report.CreatedAt = DateTime.UtcNow;

        report = await rangerReportRepository.InsertAsync(report, cancellationToken);

        return report;
    }

    public async Task<PagedResult<RangerReport>> GetByAnimalAsync(int animalId, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(animalId, cancellationToken), animalId, "Animal");

        return await rangerReportRepository.Query()
            .Where(x => x.AnimalId == animalId)
            .OrderByDescending(x => x.CreatedAt)
            .ToPagedResultAsync(pagination, cancellationToken);
    }
}
