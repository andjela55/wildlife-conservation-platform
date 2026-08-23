using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/collars")]
[Permission(PermissionCode.CollarsRead)]
public class CollarsController(ICollarService collarService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CollarResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var collars = await collarService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Collar, CollarResponseDto>(collars));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CollarResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var collar = await collarService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<CollarResponseDto>(collar));
    }

    [HttpPost]
    [Permission(PermissionCode.CollarsWrite)]
    public async Task<ActionResult<CollarResponseDto>> Create(UpsertCollarDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<CollarResponseDto>(await collarService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Permission(PermissionCode.CollarsWrite)]
    public async Task<ActionResult<CollarResponseDto>> Update(int id, UpsertCollarDto dto, CancellationToken cancellationToken)
    {
        var collar = await collarService.UpdateAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<CollarResponseDto>(collar));
    }
}
