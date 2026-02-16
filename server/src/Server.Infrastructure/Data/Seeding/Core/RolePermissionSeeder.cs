using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class RolePermissionSeeder : BaseDataSeeder
{
    public override int Order => 4;
    public override string Name => "Role Permission Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.RolePermissions.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        var systemAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var systemUserId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Admin user created in UserSeeder

        // Get all permissions and roles
        List<Permission> permissions = await context.Permissions.ToListAsync();
        List<Role> roles = await context.Roles.ToListAsync();

        Role systemAdminRole = roles.First(r => r.Id == systemAdminId);
        Role managerRole = roles.First(r => r.Name == "Manager");
        Role technicianRole = roles.First(r => r.Name == "Technician");
        Role viewerRole = roles.First(r => r.Name == "Viewer");
        Role clientRole = roles.First(r => r.Name == "Client");

        var rolePermissions = new List<RolePermission>();

        // System Administrator - All permissions
        foreach (Permission permission in permissions)
            rolePermissions.Add(new RolePermission
            {
                RoleId = systemAdminRole.Id, PermissionId = permission.Id, AssignedBy = systemUserId
            });

        // Manager - Management permissions
        string[] managerPermissionNames = new[]
        {
            "users.read", "users.update", "users.manage_roles", "clients.create", "clients.read", "clients.update",
            "clients.delete", "roles.read", "permissions.read", "reports.read", "reports.export",
            "service_orders.create", "service_orders.read", "service_orders.update", "service_orders.delete",
            "service_orders.assign", "devices.create", "devices.read", "devices.update", "devices.delete",
            "dashboard.read"
        };

        foreach (string permissionName in managerPermissionNames)
        {
            Permission? permission = permissions.FirstOrDefault(p => p.FullPermission == permissionName);
            if (permission != null)
                rolePermissions.Add(new RolePermission
                {
                    RoleId = managerRole.Id, PermissionId = permission.Id, AssignedBy = systemUserId
                });
        }

        // Technician - Service orders and devices
        string[] technicianPermissionNames = new[]
        {
            "users.read", "clients.read", "clients.update", "devices.read", "devices.update", "service_orders.read",
            "service_orders.update", "service_orders.assign", "dashboard.read"
        };

        foreach (string permissionName in technicianPermissionNames)
        {
            Permission? permission = permissions.FirstOrDefault(p => p.FullPermission == permissionName);
            if (permission != null)
                rolePermissions.Add(new RolePermission
                {
                    RoleId = technicianRole.Id, PermissionId = permission.Id, AssignedBy = systemUserId
                });
        }

        // Viewer - Read only
        string[] viewerPermissionNames = new[]
        {
            "users.read", "clients.read", "devices.read", "service_orders.read", "dashboard.read"
        };

        foreach (string permissionName in viewerPermissionNames)
        {
            Permission? permission = permissions.FirstOrDefault(p => p.FullPermission == permissionName);
            if (permission != null)
                rolePermissions.Add(new RolePermission
                {
                    RoleId = viewerRole.Id, PermissionId = permission.Id, AssignedBy = systemUserId
                });
        }

        // Client - Limited access
        string[] clientPermissionNames = new[] { "service_orders.read", "devices.read", "dashboard.read" };

        foreach (string permissionName in clientPermissionNames)
        {
            Permission? permission = permissions.FirstOrDefault(p => p.FullPermission == permissionName);
            if (permission != null)
                rolePermissions.Add(new RolePermission
                {
                    RoleId = clientRole.Id, PermissionId = permission.Id, AssignedBy = systemUserId
                });
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();

        Log.Information("✅ Created {Count} role-permission assignments", rolePermissions.Count);
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
