namespace WildlifeConservation.Services.Animals;

public interface IAnimalValidationService
{
    Task<Animal> GetRequiredAsync(int id, CancellationToken cancellationToken);
    Task ValidateUpsertAsync(UpsertAnimalDto dto, CancellationToken cancellationToken);
}
