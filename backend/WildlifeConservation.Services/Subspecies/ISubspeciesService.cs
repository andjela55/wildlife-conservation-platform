namespace WildlifeConservation.Services.Subspecies;

public interface ISubspeciesService
{
    Task<PagedResult<Models.Subspecies.Subspecies>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> CreateAsync(UpsertSubspeciesDto dto, CancellationToken cancellationToken = default);
    Task<Models.Subspecies.Subspecies> UpdateAsync(int id, UpsertSubspeciesDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
