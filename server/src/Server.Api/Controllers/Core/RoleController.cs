using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleUseCase roleUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission("roles", "read")]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
    {
        IEnumerable<RoleResponse> roles = await roleUseCase.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("roles", "read")]
    public async Task<ActionResult<RoleResponse>> GetRole(Guid id)
    {
        RoleResponse? role = await roleUseCase.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound("Role not found");

        return Ok(role);
    }

    [HttpPost]
    [RequirePermission("roles", "create")]
    public async Task<ActionResult<RoleResponse>> CreateRole([FromBody] CreateRoleRequest request)
    {
        Guid? currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
            return Unauthorized();

        RoleResponse role = await roleUseCase.CreateRoleAsync(request, currentUserId.Value);
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("roles", "update")]
    public async Task<ActionResult<RoleResponse>> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        Guid? currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
            return Unauthorized();

        RoleResponse role = await roleUseCase.UpdateRoleAsync(id, request, currentUserId.Value);
        return Ok(role);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("roles", "delete")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await roleUseCase.DeleteRoleAsync(id);
        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }
}
