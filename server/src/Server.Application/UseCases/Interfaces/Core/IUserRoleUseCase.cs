using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

/// <summary>
///     Application use case contract for assigning and querying user roles.
/// </summary>
public interface IUserRoleUseCase
{
    /// <summary>
    ///     Retrieves roles and effective permissions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user role response DTO, or <c>null</c> when user is not found.</returns>
    Task<UserRoleResponse?> GetUserRolesAsync(Guid userId);

    /// <summary>
    ///     Assigns roles to a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="request">The role assignment request.</param>
    /// <param name="currentUserId">The identifier of the user performing the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AssignRolesToUserAsync(Guid userId, AssignRoleRequest request, Guid currentUserId);

    /// <summary>
    ///     Removes a role from a user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
