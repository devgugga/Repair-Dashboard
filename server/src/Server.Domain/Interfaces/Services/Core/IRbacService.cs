using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Services.Core;

/// <summary>
///     Domain service contract for role-based access control (RBAC) operations.
/// </summary>
public interface IRbacService
{
    /// <summary>
    ///     Retrieves all roles assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of roles assigned to the user.</returns>
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);

    /// <summary>
    ///     Retrieves all effective permissions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of permissions granted to the user.</returns>
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    ///     Checks whether a user has a specific permission.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="resource">The protected resource.</param>
    /// <param name="action">The required action.</param>
    /// <returns><c>true</c> when the user has the permission; otherwise <c>false</c>.</returns>
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);

    /// <summary>
    ///     Checks whether a user has a specific role.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="roleName">The role name.</param>
    /// <returns><c>true</c> when the user has the role; otherwise <c>false</c>.</returns>
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);

    /// <summary>
    ///     Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="assignedBy">The identifier of the user performing the assignment.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy);

    /// <summary>
    ///     Removes a role from a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);

    /// <summary>
    ///     Retrieves role names assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of role names.</returns>
    Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId);

    /// <summary>
    ///     Retrieves permission names assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of permission names.</returns>
    Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId);
}
