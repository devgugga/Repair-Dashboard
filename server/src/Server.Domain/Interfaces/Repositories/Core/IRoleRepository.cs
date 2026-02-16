using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetWithPermissionsAsync();
    Task<Role?> GetWithPermissionsAsync(Guid id);
    Task<bool> ExistsAsync(string name, Guid? excludeId = null);
    Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids);
}
