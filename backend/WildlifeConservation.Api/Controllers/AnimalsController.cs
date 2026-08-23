using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/animals")]
[Permission(PermissionCode.AnimalsRead)]
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
    [Permission(PermissionCode.AnimalsWrite)]
    public async Task<ActionResult<AnimalResponseDto>> Create(UpsertAnimalDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<AnimalResponseDto>(await animalService.CreateAsync(dto, cancellationToken));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Permission(PermissionCode.AnimalsWrite)]
    public async Task<ActionResult<AnimalResponseDto>> Update(int id, UpsertAnimalDto dto, CancellationToken cancellationToken)
    {
        var animal = await animalService.UpdateAsync(id, dto, cancellationToken);
        return Ok(mapper.Map<AnimalResponseDto>(animal));
    }

}
