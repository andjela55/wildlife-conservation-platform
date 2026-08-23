using Microsoft.AspNetCore.Mvc;
using WildlifeConservation.Models.Users;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/users")]
[Permission(PermissionCode.UsersWrite)]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserResponseDto>>> GetAll([FromQuery] PaginationQuery pagination, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(pagination, cancellationToken);
        return Ok(mapper.MapPagedResult<User, UserResponseDto>(users));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto, CancellationToken cancellationToken)
    {
        var created = mapper.Map<UserResponseDto>(await userService.CreateAsync(dto, User.GetCurrentUserId(), cancellationToken));
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        return Ok(mapper.Map<UserResponseDto>(await userService.UpdateAsync(id, dto, User.GetCurrentUserId(), cancellationToken)));
    }

    [HttpPut("{id:int}/assigned-area")]
    public async Task<ActionResult<UserResponseDto>> UpdateAssignedArea(int id, UpdateUserAssignedAreaDto dto, CancellationToken cancellationToken)
    {
        var user = await userService.UpdateAssignedAreaAsync(id, dto, User.GetCurrentUserId(), cancellationToken);
        return Ok(mapper.Map<UserResponseDto>(user));
    }
}
