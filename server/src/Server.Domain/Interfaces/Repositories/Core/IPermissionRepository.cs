using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<Permission?> GetByResourceAndActionAsync(string resource, string action);
}
