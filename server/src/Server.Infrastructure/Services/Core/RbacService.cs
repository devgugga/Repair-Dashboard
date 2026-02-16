using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Domain.Interfaces.Services.Core;

namespace Server.Infrastructure.Services.Core;

public class RbacService(IUserRepository userRepository, IUserRoleRepository userRoleRepository) : IRbacService
{
    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
    {
        return await userRepository.GetUserRolesAsync(userId);
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        return await userRepository.GetUserPermissionsAsync(userId);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action)
    {
        return await userRepository.UserHasPermissionAsync(userId, resource, action);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
    {
        return await userRepository.UserHasRoleAsync(userId, roleName);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy)
    {
        await userRoleRepository.AssignRoleToUserAsync(userId, roleId, assignedBy);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        await userRoleRepository.RemoveRoleFromUserAsync(userId, roleId);
    }

    public async Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId)
    {
        return await userRepository.GetUserRoleNamesAsync(userId);
    }

    public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId)
    {
        return await userRepository.GetUserPermissionNamesAsync(userId);
    }
}
