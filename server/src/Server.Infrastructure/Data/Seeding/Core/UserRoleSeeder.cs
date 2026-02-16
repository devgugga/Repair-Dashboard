using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class UserRoleSeeder : BaseDataSeeder
{
    public override int Order => 5;
    public override string Name => "User Role Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.UserRoles.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        var systemAdminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Admin user from UserSeeder
        var systemAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // System Admin role

        // Assign System Administrator role to admin user
        var userRole = new UserRole
        {
            UserId = systemAdminUserId, RoleId = systemAdminRoleId, AssignedBy = systemAdminUserId // Self-assigned
        };

        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        Log.Information("✅ Assigned System Administrator role to admin user");
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
