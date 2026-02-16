using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

[ApiController]
[Route("api/users/{userId:guid}/roles")]
public class UserRoleController(IUserRoleUseCase userRoleUseCase) : ControllerBase
{
    [HttpGet("")]
    [RequirePermission("users", "read")]
    public async Task<ActionResult<UserRoleResponse>> GetUserRoles(Guid userId)
    {
        UserRoleResponse? userRoles = await userRoleUseCase.GetUserRolesAsync(userId);
        if (userRoles == null)
            return NotFound("User not found");

        return Ok(userRoles);
    }

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

    [HttpDelete("{roleId:guid}")]
    [RequirePermission("users", "manage_roles")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        await userRoleUseCase.RemoveRoleFromUserAsync(userId, roleId);
        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }
}
