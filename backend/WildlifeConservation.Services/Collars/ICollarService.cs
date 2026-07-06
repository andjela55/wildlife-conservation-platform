namespace WildlifeConservation.Services.Collars;

public interface ICollarService
{
    Task<PagedResult<Collar>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default);
    Task<Collar> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Collar> CreateAsync(CreateCollarDto dto, CancellationToken cancellationToken = default);
    Task<Collar> UpdateAsync(int id, UpdateCollarDto dto, CancellationToken cancellationToken = default);
}
