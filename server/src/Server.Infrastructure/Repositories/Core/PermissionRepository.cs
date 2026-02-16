using Microsoft.EntityFrameworkCore;

using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories.Core;

public class PermissionRepository(ServerDbContext context)
    : BaseRepository<Permission>(context), IPermissionRepository
{
    private readonly ServerDbContext _context = context;

    public async Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Permissions
            .Where(p => ids.Contains(p.Id) && p.IsActive)
            .ToListAsync();
    }

    public async Task<Permission?> GetByResourceAndActionAsync(string resource, string action)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Resource == resource && p.Action == action && p.IsActive);
    }
}
