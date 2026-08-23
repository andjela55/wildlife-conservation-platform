namespace WildlifeConservation.Services.Species;

public interface ISpeciesService
{
    Task<PagedResult<Models.Species.Species>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> CreateAsync(UpsertSpeciesDto dto, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> UpdateAsync(int id, UpsertSpeciesDto dto, CancellationToken cancellationToken = default);
}
