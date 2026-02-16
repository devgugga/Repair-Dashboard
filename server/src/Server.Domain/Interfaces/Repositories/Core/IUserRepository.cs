using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Repositories.Core;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetWithPersonAsync(Guid id);
    Task<User?> GetWithRolesAsync(Guid id);
    Task<User?> GetWithRolesAndPermissionsAsync(Guid id);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId);
    Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId);
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);
}
