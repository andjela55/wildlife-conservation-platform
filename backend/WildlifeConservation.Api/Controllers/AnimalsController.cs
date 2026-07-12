using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/animals")]
[AuthorizeRoles(UserRole.Admin, UserRole.Ranger, UserRole.Researcher)]
public class AnimalsController(IAnimalService animalService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AnimalResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var animals = await animalService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Animal, AnimalResponseDto>(animals));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnimalResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var animal = await animalService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<AnimalResponseDto>(animal));
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin, UserRole.Researcher)]
    public async Task<ActionResult<AnimalResponseDto>> Create(CreateAnimalDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<AnimalResponseDto>(await animalService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Admin, UserRole.Researcher)]
    public async Task<ActionResult<AnimalResponseDto>> Update(int id, UpdateAnimalDto dto, CancellationToken cancellationToken)
    {
        var animal = await animalService.UpdateAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<AnimalResponseDto>(animal));
    }

    [HttpGet("{id:int}/locations")]
    public async Task<ActionResult<PagedResult<LocationPointResponseDto>>> GetLocations(int id, [FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var locations = await animalService.GetLocationsAsync(id, pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<LocationPoint, LocationPointResponseDto>(locations));
    }

    [HttpGet("{id:int}/reports")]
    public async Task<ActionResult<PagedResult<RangerReportResponseDto>>> GetReports(int id, [FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var reports = await animalService.GetReportsAsync(id, pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<RangerReport, RangerReportResponseDto>(reports));
    }

    [HttpGet("{id:int}/alerts")]
    public async Task<ActionResult<PagedResult<AlertResponseDto>>> GetAlerts(int id, [FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var alerts = await animalService.GetAlertsAsync(id, pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<Alert, AlertResponseDto>(alerts));
    }
}
