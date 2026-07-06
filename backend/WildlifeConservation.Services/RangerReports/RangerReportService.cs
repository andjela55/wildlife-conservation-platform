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

    public async Task<RangerReport> CreateAsync(CreateRangerReportDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.AnimalId.HasValue)
        {
            await ServiceHelpers.EnsureFoundAsync(animalRepository.GetByIdAsync(dto.AnimalId.Value, cancellationToken), dto.AnimalId.Value, "Animal");
        }

        await ServiceHelpers.EnsureFoundAsync(userRepository.GetByIdAsync(dto.UserId, cancellationToken), dto.UserId, "User");

        var report = mapper.Map<RangerReport>(dto);
        report.Description = ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        report.CreatedAt = ServiceHelpers.AsUtc(dto.CreatedAt);

        report = await rangerReportRepository.InsertAsync(report, cancellationToken);

        return report;
    }
}
