using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

/// <summary>
///     Application use case contract for role management operations.
/// </summary>
public interface IRoleUseCase
{
    /// <summary>
    ///     Retrieves all roles with their details.
    /// </summary>
    /// <returns>A collection of role response DTOs.</returns>
    Task<IEnumerable<RoleResponse>> GetAllRolesAsync();

    /// <summary>
    ///     Retrieves a role by identifier.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <returns>The role response DTO, or <c>null</c> when not found.</returns>
    Task<RoleResponse?> GetRoleByIdAsync(Guid id);

    /// <summary>
    ///     Creates a new role.
    /// </summary>
    /// <param name="request">The role creation request.</param>
    /// <param name="currentUserId">The identifier of the user performing the operation.</param>
    /// <returns>The created role response DTO.</returns>
    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, Guid currentUserId);

    /// <summary>
    ///     Updates an existing role.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <param name="request">The role update request.</param>
    /// <param name="currentUserId">The identifier of the user performing the operation.</param>
    /// <returns>The updated role response DTO.</returns>
    Task<RoleResponse> UpdateRoleAsync(Guid id, UpdateRoleRequest request, Guid currentUserId);

    /// <summary>
    ///     Soft-deletes a role.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteRoleAsync(Guid id);
}
