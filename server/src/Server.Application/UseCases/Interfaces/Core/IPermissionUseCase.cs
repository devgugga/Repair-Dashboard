using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

/// <summary>
///     Application use case contract for permission read operations.
/// </summary>
public interface IPermissionUseCase
{
    /// <summary>
    ///     Retrieves all permissions.
    /// </summary>
    /// <returns>A collection of permission response DTOs.</returns>
    Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync();

    /// <summary>
    ///     Retrieves a permission by identifier.
    /// </summary>
    /// <param name="id">The permission identifier.</param>
    /// <returns>The permission response DTO, or <c>null</c> when not found.</returns>
    Task<PermissionResponse?> GetPermissionByIdAsync(Guid id);
}
