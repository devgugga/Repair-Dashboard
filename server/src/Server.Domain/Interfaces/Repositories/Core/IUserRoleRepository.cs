using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
