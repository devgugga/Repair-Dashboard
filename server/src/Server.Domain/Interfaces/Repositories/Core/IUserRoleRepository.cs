using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

/// <summary>
///     Repository contract for user-role assignments.
/// </summary>
public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    /// <summary>
    ///     Retrieves a user-role assignment by user and role identifiers.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>The user-role assignment or <c>null</c> when not found.</returns>
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId);

    /// <summary>
    ///     Retrieves all role assignments for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of user-role assignments.</returns>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);

    /// <summary>
    ///     Retrieves active role assignments for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of active user-role assignments.</returns>
    Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId);

    /// <summary>
    ///     Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="assignedBy">The identifier of the user performing the assignment.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy);

    /// <summary>
    ///     Removes a role assignment from a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
