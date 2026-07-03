namespace WildlifeConservation.Services.Subspecies;

public interface ISubspeciesService
{
    Task<List<Models.Subspecies.Subspecies>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> CreateAsync(CreateSubspeciesDto dto, CancellationToken cancellationToken = default);
}
