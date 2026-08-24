using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/species")]
[Permission(PermissionCode.SpeciesRead)]
public class SpeciesController(ISpeciesService speciesService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SpeciesResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var species = await speciesService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Species, SpeciesResponseDto>(species));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeciesResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var species = await speciesService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<SpeciesResponseDto>(species));
    }

    [HttpPost]
    [Permission(PermissionCode.SpeciesWrite)]
    public async Task<ActionResult<SpeciesResponseDto>> Create(UpsertSpeciesDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<SpeciesResponseDto>(await speciesService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Permission(PermissionCode.SpeciesWrite)]
    public async Task<ActionResult<SpeciesResponseDto>> Update(int id, UpsertSpeciesDto dto, CancellationToken cancellationToken)
    {
        return Ok(mapper.Map<SpeciesResponseDto>(await speciesService.UpdateAsync(id, dto, cancellationToken)));
    }

    [HttpDelete("{id:int}")]
    [Permission(PermissionCode.SpeciesWrite)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await speciesService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
