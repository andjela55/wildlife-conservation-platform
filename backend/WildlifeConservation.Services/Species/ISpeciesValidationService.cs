namespace WildlifeConservation.Services.Species;

public interface ISpeciesValidationService
{
    Task<Models.Species.Species> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task ValidateUpsertAsync(UpsertSpeciesDto dto, int? existingId, CancellationToken cancellationToken);
}
