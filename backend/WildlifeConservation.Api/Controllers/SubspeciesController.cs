using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/subspecies")]
[AuthorizeRoles(UserRole.Admin, UserRole.Researcher)]
public class SubspeciesController(ISubspeciesService subspeciesService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<SubspeciesResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var subspecies = await subspeciesService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Subspecies, SubspeciesResponseDto>(subspecies));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubspeciesResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var subspecies = await subspeciesService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<SubspeciesResponseDto>(subspecies));
    }

    [HttpPost]
    public async Task<ActionResult<SubspeciesResponseDto>> Create(CreateSubspeciesDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<SubspeciesResponseDto>(await subspeciesService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
