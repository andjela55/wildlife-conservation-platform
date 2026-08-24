using Microsoft.AspNetCore.Mvc;

namespace WildlifeConservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Permission(PermissionCode.RolesRead)]
public class RolesController(IRoleService roleService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RoleResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAllAsync(cancellationToken);
        return Ok(mapper.Map<IReadOnlyCollection<RoleResponseDto>>(roles));
    }
}
