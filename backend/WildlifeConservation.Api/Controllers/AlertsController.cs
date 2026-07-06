using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController(IAlertService alertService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AlertResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var alerts = await alertService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Alert, AlertResponseDto>(alerts));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlertResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var alert = await alertService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<AlertResponseDto>(alert));
    }

    [HttpPost]
    public async Task<ActionResult<AlertResponseDto>> Create(CreateAlertDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<AlertResponseDto>(await alertService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/resolve")]
    public async Task<ActionResult<AlertResponseDto>> Resolve(int id, ResolveAlertDto dto, CancellationToken cancellationToken)
    {
        var alert = await alertService.ResolveAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<AlertResponseDto>(alert));
    }
}
