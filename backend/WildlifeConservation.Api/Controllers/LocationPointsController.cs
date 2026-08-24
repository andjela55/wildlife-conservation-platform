using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/location-points")]
public class LocationPointsController(ILocationPointService locationPointService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [AuthorizeDevice]
    public async Task<ActionResult<LocationPointResponseDto>> Create(CreateLocationPointDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<LocationPointResponseDto>(await locationPointService.CreateAsync(dto, cancellationToken));
        return Created($"/api/location-points/by-animal/{created.AnimalId}", created);
    }

    [HttpGet("latest")]
    [Permission(PermissionCode.LocationPointsRead)]
    public async Task<ActionResult<PagedResult<LocationPointResponseDto>>> GetLatest([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var locations = await locationPointService.GetLatestAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<LocationPoint, LocationPointResponseDto>(locations));
    }

    [HttpGet("by-animal/{animalId:int}")]
    [Permission(PermissionCode.LocationPointsRead)]
    public async Task<ActionResult<PagedResult<LocationPointResponseDto>>> GetByAnimal(int animalId, [FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var locations = await locationPointService.GetByAnimalAsync(animalId, pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<LocationPoint, LocationPointResponseDto>(locations));
    }
}
