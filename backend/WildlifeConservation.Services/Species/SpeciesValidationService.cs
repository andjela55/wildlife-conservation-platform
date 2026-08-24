namespace WildlifeConservation.Services.Species;

public class SpeciesValidationService(ISpeciesRepository speciesRepository) : ISpeciesValidationService
{
    public async Task<Models.Species.Species> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await speciesRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Species with id {id} was not found.");

    public async Task ValidateUpsertAsync(UpsertSpeciesDto dto, int? existingId, CancellationToken cancellationToken)
    {
        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        var duplicate = await speciesRepository.Query().AnyAsync(
            x => (!existingId.HasValue || x.Id != existingId.Value) && x.Name.ToLower() == name.ToLower(),
            cancellationToken);
        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Species '{name}' already exists.");
        }
    }
}
