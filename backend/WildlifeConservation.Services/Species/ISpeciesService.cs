namespace WildlifeConservation.Services.Species;

public interface ISpeciesService
{
    Task<PagedResult<Models.Species.Species>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> CreateAsync(CreateSpeciesDto dto, CancellationToken cancellationToken = default);
}
