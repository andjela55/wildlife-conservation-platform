using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/ranger-reports")]
public class RangerReportsController(IRangerReportService rangerReportService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<RangerReportResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var reports = await rangerReportService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<RangerReport, RangerReportResponseDto>(reports));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RangerReportResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var report = await rangerReportService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<RangerReportResponseDto>(report));
    }

    [HttpPost]
    public async Task<ActionResult<RangerReportResponseDto>> Create(CreateRangerReportDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<RangerReportResponseDto>(await rangerReportService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
