namespace WildlifeConservation.Services.Subspecies;

public interface ISubspeciesValidationService
{
    Task<Models.Subspecies.Subspecies> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task ValidateUpsertAsync(UpsertSubspeciesDto dto, int? existingId, CancellationToken cancellationToken);
}
