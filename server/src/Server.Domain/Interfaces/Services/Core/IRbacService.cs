using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Services.Core;

public interface IRbacService
{
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId);
    Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId);
}
