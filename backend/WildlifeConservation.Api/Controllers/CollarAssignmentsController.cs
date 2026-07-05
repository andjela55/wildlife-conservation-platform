using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/collar-assignments")]
public class CollarAssignmentsController(ICollarAssignmentService collarAssignmentService, IMapper mapper) : ControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<List<CollarAssignmentResponseDto>>> GetActive(CancellationToken cancellationToken)
    {
        var assignments = await collarAssignmentService.GetActiveAsync(cancellationToken);
        return Ok(mapper.Map<List<CollarAssignmentResponseDto>>(assignments));
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
