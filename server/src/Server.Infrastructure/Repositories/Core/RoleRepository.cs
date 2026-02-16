using Microsoft.EntityFrameworkCore;

using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories.Core;

public class RoleRepository(ServerDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    private readonly ServerDbContext _context = context;

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name && r.IsActive);
    }

    public async Task<IEnumerable<Role>> GetWithPermissionsAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<Role?> GetWithPermissionsAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
    }

    public async Task<bool> ExistsAsync(string name, Guid? excludeId = null)
    {
        IQueryable<Role> query = _context.Roles.Where(r => r.Name == name && r.IsActive);

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Roles
            .Where(r => ids.Contains(r.Id) && r.IsActive)
            .ToListAsync();
    }
}
