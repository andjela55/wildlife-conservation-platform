using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/animals")]
public class AnimalsController(IAnimalService animalService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AnimalResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var animals = await animalService.GetAllAsync(cancellationToken);
        return Ok(mapper.Map<List<AnimalResponseDto>>(animals));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnimalResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var animal = await animalService.GetByIdAsync(id, cancellationToken);
        return Ok(mapper.Map<AnimalResponseDto>(animal));
    }

    [HttpPost]
    public async Task<ActionResult<AnimalResponseDto>> Create(CreateAnimalDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<AnimalResponseDto>(await animalService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AnimalResponseDto>> Update(int id, UpdateAnimalDto dto, CancellationToken cancellationToken)
    {
        var animal = await animalService.UpdateAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<AnimalResponseDto>(animal));
    }

    [HttpGet("{id:int}/locations")]
    public async Task<ActionResult<List<LocationPointResponseDto>>> GetLocations(int id, CancellationToken cancellationToken)
    {
        var locations = await animalService.GetLocationsAsync(id, cancellationToken);
        return Ok(mapper.Map<List<LocationPointResponseDto>>(locations));
    }

    [HttpGet("{id:int}/reports")]
    public async Task<ActionResult<List<RangerReportResponseDto>>> GetReports(int id, CancellationToken cancellationToken)
    {
        var reports = await animalService.GetReportsAsync(id, cancellationToken);
        return Ok(mapper.Map<List<RangerReportResponseDto>>(reports));
    }

    [HttpGet("{id:int}/alerts")]
    public async Task<ActionResult<List<AlertResponseDto>>> GetAlerts(int id, CancellationToken cancellationToken)
    {
        var alerts = await animalService.GetAlertsAsync(id, cancellationToken);
        return Ok(mapper.Map<List<AlertResponseDto>>(alerts));
    }
}
