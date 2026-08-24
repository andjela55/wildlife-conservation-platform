namespace WildlifeConservation.Services.Collars;

public interface ICollarValidationService
{
    Task<Collar> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task ValidateCreateAsync(UpsertCollarDto dto, CancellationToken cancellationToken);
    Task<Collar> ValidateUpdateAsync(int id, UpsertCollarDto dto, CancellationToken cancellationToken);
}
