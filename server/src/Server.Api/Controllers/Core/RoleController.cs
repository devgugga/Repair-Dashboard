using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

/// <summary>
///     Handles role management endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleUseCase roleUseCase) : ControllerBase
{
    /// <summary>
    ///     Retrieves all roles.
    /// </summary>
    /// <returns>A list of roles.</returns>
    [HttpGet]
    [RequirePermission("roles", "read")]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
    {
        IEnumerable<RoleResponse> roles = await roleUseCase.GetAllRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    ///     Retrieves a role by identifier.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <returns>The role when found.</returns>
    [HttpGet("{id:guid}")]
    [RequirePermission("roles", "read")]
    public async Task<ActionResult<RoleResponse>> GetRole(Guid id)
    {
        RoleResponse? role = await roleUseCase.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound("Role not found");

        return Ok(role);
    }

    /// <summary>
    ///     Creates a role.
    /// </summary>
    /// <param name="request">The role creation payload.</param>
    /// <returns>The created role.</returns>
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

    /// <summary>
    ///     Updates an existing role.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <param name="request">The role update payload.</param>
    /// <returns>The updated role.</returns>
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

    /// <summary>
    ///     Deletes a role.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{id:guid}")]
    [RequirePermission("roles", "delete")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await roleUseCase.DeleteRoleAsync(id);
        return NoContent();
    }

    /// <summary>
    ///     Reads the current user id from claims.
    /// </summary>
    /// <returns>The current user id, or <c>null</c> when unavailable.</returns>
    private Guid? GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }
}
