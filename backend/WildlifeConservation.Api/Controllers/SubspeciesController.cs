using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/subspecies")]
[Permission(PermissionCode.SubspeciesRead)]
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
    [Permission(PermissionCode.SubspeciesWrite)]
    public async Task<ActionResult<SubspeciesResponseDto>> Create(UpsertSubspeciesDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<SubspeciesResponseDto>(await subspeciesService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Permission(PermissionCode.SubspeciesWrite)]
    public async Task<ActionResult<SubspeciesResponseDto>> Update(int id, UpsertSubspeciesDto dto, CancellationToken cancellationToken)
    {
        return Ok(mapper.Map<SubspeciesResponseDto>(await subspeciesService.UpdateAsync(id, dto, cancellationToken)));
    }
}
