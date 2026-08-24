namespace WildlifeConservation.Services.LocationPoints;

public class LocationPointValidationService(
    IAnimalRepository animalRepository,
    ICollarRepository collarRepository,
    ICollarAssignmentRepository collarAssignmentRepository) : ILocationPointValidationService
{
    public async Task<(Animal Animal, Collar Collar)> ValidateCreateAsync(
        CreateLocationPointDto dto,
        CancellationToken cancellationToken)
    {
        var animal = await ServiceHelpers.EnsureFoundAsync(
            animalRepository.GetByIdAsync(dto.AnimalId, cancellationToken), dto.AnimalId, "Animal");
        var collar = await ServiceHelpers.EnsureFoundAsync(
            collarRepository.GetByIdAsync(dto.CollarId, cancellationToken), dto.CollarId, "Collar");
        var recordedAt = ServiceHelpers.AsUtc(dto.RecordedAt);
        var collarWasAssignedToAnimal = await collarAssignmentRepository.Query()
            .AnyAsync(x =>
                x.AnimalId == dto.AnimalId &&
                x.CollarId == dto.CollarId &&
                x.AssignedAt <= recordedAt &&
                (x.UnassignedAt == null || x.UnassignedAt >= recordedAt),
                cancellationToken);

        if (!collarWasAssignedToAnimal)
        {
            throw new ServiceException(
                (int)HttpStatusCode.BadRequest,
                "Location points can only be recorded from a collar assigned to the animal at the recorded time.");
        }

        return (animal, collar);
    }

    public async Task ValidateAnimalAsync(int animalId, CancellationToken cancellationToken)
    {
        await ServiceHelpers.EnsureFoundAsync(
            animalRepository.GetByIdAsync(animalId, cancellationToken), animalId, "Animal");
    }
}
