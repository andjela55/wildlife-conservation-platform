namespace WildlifeConservation.Services.Subspecies;

public class SubspeciesService(
    ISubspeciesRepository subspeciesRepository,
    ISubspeciesValidationService validationService,
    IMapper mapper) : ISubspeciesService
{
    public async Task<PagedResult<Models.Subspecies.Subspecies>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await subspeciesRepository.Query()
            .OrderBy(x => x.Name)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<Models.Subspecies.Subspecies> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await validationService.GetRequiredAsync(id, cancellationToken);
    }

    public async Task<Models.Subspecies.Subspecies> CreateAsync(UpsertSubspeciesDto dto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateUpsertAsync(dto, existingId: null, cancellationToken);

        var subspecies = mapper.Map<Models.Subspecies.Subspecies>(dto);

        subspecies = await subspeciesRepository.InsertAsync(subspecies, cancellationToken);

        return subspecies;
    }

    public async Task<Models.Subspecies.Subspecies> UpdateAsync(int id, UpsertSubspeciesDto dto, CancellationToken cancellationToken = default)
    {
        var subspecies = await validationService.GetRequiredAsync(id, cancellationToken);
        await validationService.ValidateUpsertAsync(dto, id, cancellationToken);

        mapper.Map(dto, subspecies);
        return await subspeciesRepository.UpdateAsync(subspecies, cancellationToken);
    }
}
