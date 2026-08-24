namespace WildlifeConservation.Services.Animals;

public class AnimalValidationService(IAnimalRepository animalRepository, ISubspeciesRepository subspeciesRepository) : IAnimalValidationService
{
    public async Task<Animal> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await animalRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Animal with id {id} was not found.");

    public async Task ValidateUpsertAsync(UpsertAnimalDto dto, CancellationToken cancellationToken)
    {
        ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        await ServiceHelpers.EnsureFoundAsync(
            subspeciesRepository.GetByIdAsync(dto.SubspeciesId, cancellationToken), dto.SubspeciesId, "Subspecies");
    }
}
