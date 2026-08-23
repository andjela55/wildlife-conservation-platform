namespace WildlifeConservation.Services.Subspecies;

public class SubspeciesValidationService(
    ISubspeciesRepository subspeciesRepository,
    ISpeciesRepository speciesRepository) : ISubspeciesValidationService
{
    public async Task<Models.Subspecies.Subspecies> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await subspeciesRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"Subspecies with id {id} was not found.");

    public async Task ValidateUpsertAsync(UpsertSubspeciesDto dto, int? existingId, CancellationToken cancellationToken)
    {
        await ServiceHelpers.EnsureFoundAsync(
            speciesRepository.GetByIdAsync(dto.SpeciesId, cancellationToken), dto.SpeciesId, "Species");
        var name = ServiceHelpers.RequiredText(dto.Name, nameof(dto.Name));
        ServiceHelpers.RequiredText(dto.Description, nameof(dto.Description));
        var duplicate = await subspeciesRepository.Query().AnyAsync(
            x => (!existingId.HasValue || x.Id != existingId.Value) &&
                 x.SpeciesId == dto.SpeciesId && x.Name.ToLower() == name.ToLower(),
            cancellationToken);
        if (duplicate)
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"Subspecies '{name}' already exists for this species.");
        }
    }
}
