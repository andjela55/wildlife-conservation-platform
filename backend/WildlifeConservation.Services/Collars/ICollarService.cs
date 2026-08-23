namespace WildlifeConservation.Services.Collars;

public interface ICollarService
{
    Task<PagedResult<Collar>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Collar> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Collar> CreateAsync(UpsertCollarDto dto, CancellationToken cancellationToken = default);
    Task<Collar> UpdateAsync(int id, UpsertCollarDto dto, CancellationToken cancellationToken = default);
}
