namespace WildlifeConservation.Services.LocationPoints;

public interface ILocationPointValidationService
{
    Task<(Animal Animal, Collar Collar)> ValidateCreateAsync(
        CreateLocationPointDto dto,
        CancellationToken cancellationToken);
    Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken);
}
