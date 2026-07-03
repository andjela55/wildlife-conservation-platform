using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/species")]
public class SpeciesController(ISpeciesService speciesService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SpeciesResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var species = await speciesService.GetAllAsync(cancellationToken);
        return Ok(mapper.Map<List<SpeciesResponseDto>>(species));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeciesResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var species = await speciesService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<SpeciesResponseDto>(species));
    }

    [HttpPost]
    public async Task<ActionResult<SpeciesResponseDto>> Create(CreateSpeciesDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<SpeciesResponseDto>(await speciesService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
