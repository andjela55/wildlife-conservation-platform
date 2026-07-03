namespace WildlifeConservation.Services.Species;

public interface ISpeciesService
{
    Task<List<Models.Species.Species>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.Species.Species> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Species.Species> CreateAsync(CreateSpeciesDto dto, CancellationToken cancellationToken = default);
}
