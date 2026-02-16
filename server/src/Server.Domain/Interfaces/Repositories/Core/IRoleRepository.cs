using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

/// <summary>
///     Repository contract for role entities.
/// </summary>
public interface IRoleRepository : IBaseRepository<Role>
{
    /// <summary>
    ///     Retrieves a role by name.
    /// </summary>
    /// <param name="name">The role name.</param>
    /// <returns>The matching role or <c>null</c> when not found.</returns>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    ///     Retrieves all roles including permission relations.
    /// </summary>
    /// <returns>A collection of roles with permissions loaded.</returns>
    Task<IEnumerable<Role>> GetWithPermissionsAsync();

    /// <summary>
    ///     Retrieves one role including permission relations.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <returns>The role with permissions or <c>null</c> when not found.</returns>
    Task<Role?> GetWithPermissionsAsync(Guid id);

    /// <summary>
    ///     Checks whether a role name already exists.
    /// </summary>
    /// <param name="name">The role name to check.</param>
    /// <param name="excludeId">Optional role id to exclude from uniqueness validation.</param>
    /// <returns><c>true</c> when the name exists; otherwise <c>false</c>.</returns>
    Task<bool> ExistsAsync(string name, Guid? excludeId = null);

    /// <summary>
    ///     Retrieves roles by identifiers.
    /// </summary>
    /// <param name="ids">The role identifiers.</param>
    /// <returns>A collection of roles.</returns>
    Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids);
}
