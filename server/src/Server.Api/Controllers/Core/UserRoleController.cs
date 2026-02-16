using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

/// <summary>
///     Handles user-role assignment endpoints.
/// </summary>
[ApiController]
[Route("api/users/{userId:guid}/roles")]
public class UserRoleController(IUserRoleUseCase userRoleUseCase) : ControllerBase
{
    /// <summary>
    ///     Retrieves assigned roles and permissions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>User role data when found.</returns>
    [HttpGet("")]
    [RequirePermission("users", "read")]
    public async Task<ActionResult<UserRoleResponse>> GetUserRoles(Guid userId)
    {
        UserRoleResponse? userRoles = await userRoleUseCase.GetUserRolesAsync(userId);
        if (userRoles == null)
            return NotFound("User not found");

        return Ok(userRoles);
    }

    /// <summary>
    ///     Assigns one or more roles to a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="request">The role assignment request.</param>
    /// <returns>No content when successful.</returns>
    [HttpPost("")]
    [RequirePermission("users", "manage_roles")]
    public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] AssignRoleRequest request)
    {
        Guid? currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
            return Unauthorized();

        await userRoleUseCase.AssignRolesToUserAsync(userId, request, currentUserId.Value);
        return NoContent();
    }

    /// <summary>
    ///     Removes a role from a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{roleId:guid}")]
    [RequirePermission("users", "manage_roles")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        await userRoleUseCase.RemoveRoleFromUserAsync(userId, roleId);
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
