using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

/// <summary>
///     Repository contract for user entities and authorization projections.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Retrieves a user by e-mail address.
    /// </summary>
    /// <param name="email">The e-mail address.</param>
    /// <returns>The matching user or <c>null</c> when not found.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    ///     Retrieves a user by username.
    /// </summary>
    /// <param name="userName">The username.</param>
    /// <returns>The matching user or <c>null</c> when not found.</returns>
    Task<User?> GetByUserNameAsync(string userName);

    /// <summary>
    ///     Retrieves a user with person information loaded.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The user with person data or <c>null</c> when not found.</returns>
    Task<User?> GetWithPersonAsync(Guid id);

    /// <summary>
    ///     Retrieves a user with roles loaded.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The user with roles or <c>null</c> when not found.</returns>
    Task<User?> GetWithRolesAsync(Guid id);

    /// <summary>
    ///     Retrieves a user with roles and permissions loaded.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The user with roles and permissions or <c>null</c> when not found.</returns>
    Task<User?> GetWithRolesAndPermissionsAsync(Guid id);

    /// <summary>
    ///     Retrieves roles assigned to a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of assigned roles.</returns>
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);

    /// <summary>
    ///     Retrieves effective permissions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of permissions.</returns>
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);

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

    /// <summary>
    ///     Checks whether a user has a role.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="roleName">The role name.</param>
    /// <returns><c>true</c> when the role is assigned; otherwise <c>false</c>.</returns>
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);

    /// <summary>
    ///     Checks whether a user has a permission.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="resource">The permission resource.</param>
    /// <param name="action">The permission action.</param>
    /// <returns><c>true</c> when the permission is assigned; otherwise <c>false</c>.</returns>
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);
}
