using Microsoft.EntityFrameworkCore;

using Server.Domain.Entities.Core;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories.Core;

public class UserRoleRepository(ServerDbContext context)
    : BaseRepository<UserRole>(context), IUserRoleRepository
{
    private readonly ServerDbContext _context = context;

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy)
    {
        // Check if assignment already exists
        UserRole? existingAssignment = await GetByUserAndRoleAsync(userId, roleId);

        if (existingAssignment != null)
        {
            if (existingAssignment.IsActive) return;

            existingAssignment.IsActive = true;
            existingAssignment.AssignedAt = DateTimeOffset.UtcNow;
            existingAssignment.AssignedBy = assignedBy;
            existingAssignment.ExpiresAt = null;
            await UpdateAsync(existingAssignment);

            return;
        }

        var userRole = new UserRole { UserId = userId, RoleId = roleId, AssignedBy = assignedBy };

        await AddAsync(userRole);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        UserRole? userRole = await GetByUserAndRoleAsync(userId, roleId);
        if (userRole != null)
        {
            userRole.IsActive = false;
            await UpdateAsync(userRole);
        }
    }
}
