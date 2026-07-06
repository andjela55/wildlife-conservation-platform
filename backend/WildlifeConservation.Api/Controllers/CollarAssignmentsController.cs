using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/collar-assignments")]
public class CollarAssignmentsController(ICollarAssignmentService collarAssignmentService, IMapper mapper) : ControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<PagedResult<CollarAssignmentResponseDto>>> GetActive([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var assignments = await collarAssignmentService.GetActiveAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<CollarAssignment, CollarAssignmentResponseDto>(assignments));
    }

    [HttpPost]
    public async Task<ActionResult<CollarAssignmentResponseDto>> Create(CreateCollarAssignmentDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<CollarAssignmentResponseDto>(await collarAssignmentService.CreateAsync(dto, cancellationToken));
        return Created($"/api/collar-assignments/{created.Id}", created);
    }

    [HttpPut("{id:int}/unassign")]
    public async Task<ActionResult<CollarAssignmentResponseDto>> Unassign(int id, UnassignCollarDto dto, CancellationToken cancellationToken)
    {
        var assignment = await collarAssignmentService.UnassignAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<CollarAssignmentResponseDto>(assignment));
    }
}
