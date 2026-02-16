using Microsoft.EntityFrameworkCore;

using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories.Core;

public class UserRepository(ServerDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private readonly ServerDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Person.Email == email && u.IsActive);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);
    }

    public async Task<User?> GetWithPersonAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<User?> GetWithRolesAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<User?> GetWithRolesAndPermissionsAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Where(p => p.IsActive)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.FullPermission)
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.Role.IsActive && ur.Role.Name == roleName);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.IsActive &&
                            rp.Permission.Resource == resource &&
                            rp.Permission.Action == action);
    }
}
