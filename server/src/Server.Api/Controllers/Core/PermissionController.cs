using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

[ApiController]
[Route("api/[controller]")]
public class PermissionController(IPermissionUseCase permissionUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission("permissions", "read")]
    public async Task<ActionResult<IEnumerable<PermissionResponse>>> GetPermissions()
    {
        IEnumerable<PermissionResponse> permissions = await permissionUseCase.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("permissions", "read")]
    public async Task<ActionResult<PermissionResponse>> GetPermission(Guid id)
    {
        PermissionResponse? permission = await permissionUseCase.GetPermissionByIdAsync(id);
        if (permission == null)
            return NotFound("Permission not found");

        return Ok(permission);
    }
}
