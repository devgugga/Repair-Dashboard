using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

public interface IUserRoleUseCase
{
    Task<UserRoleResponse?> GetUserRolesAsync(Guid userId);
    Task AssignRolesToUserAsync(Guid userId, AssignRoleRequest request, Guid currentUserId);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
