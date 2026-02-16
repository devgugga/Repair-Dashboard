using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using Server.Domain.Interfaces.Services.Core;

namespace Server.Api.Authorization;

public class PermissionAuthorizationHandler(IRbacService rbacService) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Check if user is authenticated
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }

        // Get user ID from claims
        string? userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            context.Fail();
            return;
        }

        // Check if user has the required permission
        bool hasPermission = await rbacService.UserHasPermissionAsync(
            userId,
            requirement.Resource,
            requirement.Action);

        if (hasPermission)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}
