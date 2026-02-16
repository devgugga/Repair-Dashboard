using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

/// <summary>
///     Handles permission query endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PermissionController(IPermissionUseCase permissionUseCase) : ControllerBase
{
    /// <summary>
    ///     Retrieves all permissions.
    /// </summary>
    /// <returns>A list of permissions.</returns>
    [HttpGet]
    [RequirePermission("permissions", "read")]
    public async Task<ActionResult<IEnumerable<PermissionResponse>>> GetPermissions()
    {
        IEnumerable<PermissionResponse> permissions = await permissionUseCase.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    /// <summary>
    ///     Retrieves a permission by identifier.
    /// </summary>
    /// <param name="id">The permission identifier.</param>
    /// <returns>The permission when found.</returns>
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
