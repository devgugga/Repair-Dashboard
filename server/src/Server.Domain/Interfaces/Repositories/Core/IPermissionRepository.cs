using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

/// <summary>
///     Repository contract for permission entities.
/// </summary>
public interface IPermissionRepository : IBaseRepository<Permission>
{
    /// <summary>
    ///     Retrieves permissions by identifiers.
    /// </summary>
    /// <param name="ids">The permission identifiers.</param>
    /// <returns>A collection of permissions.</returns>
    Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<Guid> ids);

    /// <summary>
    ///     Retrieves a permission by resource and action.
    /// </summary>
    /// <param name="resource">The permission resource.</param>
    /// <param name="action">The permission action.</param>
    /// <returns>The matching permission or <c>null</c> when not found.</returns>
    Task<Permission?> GetByResourceAndActionAsync(string resource, string action);
}
