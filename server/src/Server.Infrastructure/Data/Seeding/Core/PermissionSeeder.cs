using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class PermissionSeeder : BaseDataSeeder
{
    public override int Order => 2;
    public override string Name => "Permission Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.Permissions.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        var permissions = new List<Permission>
        {
            // User Management
            new()
            {
                Name = "Create Users",
                Resource = "users",
                Action = "create",
                Description = "Permite criar novos usuários"
            },
            new()
            {
                Name = "Read Users",
                Resource = "users",
                Action = "read",
                Description = "Permite visualizar usuários"
            },
            new()
            {
                Name = "Update Users",
                Resource = "users",
                Action = "update",
                Description = "Permite atualizar informações de usuários"
            },
            new()
            {
                Name = "Delete Users",
                Resource = "users",
                Action = "delete",
                Description = "Permite deletar usuários"
            },
            new()
            {
                Name = "Manage User Roles",
                Resource = "users",
                Action = "manage_roles",
                Description = "Permite gerenciar roles de usuários"
            },

            // Client Management
            new()
            {
                Name = "Create Clients",
                Resource = "clients",
                Action = "create",
                Description = "Permite criar novos clientes"
            },
            new()
            {
                Name = "Read Clients",
                Resource = "clients",
                Action = "read",
                Description = "Permite visualizar clientes"
            },
            new()
            {
                Name = "Update Clients",
                Resource = "clients",
                Action = "update",
                Description = "Permite atualizar informações de clientes"
            },
            new()
            {
                Name = "Delete Clients",
                Resource = "clients",
                Action = "delete",
                Description = "Permite deletar clientes"
            },

            // Role Management
            new()
            {
                Name = "Create Roles",
                Resource = "roles",
                Action = "create",
                Description = "Permite criar novos roles"
            },
            new()
            {
                Name = "Read Roles", Resource = "roles", Action = "read", Description = "Permite visualizar roles"
            },
            new()
            {
                Name = "Update Roles",
                Resource = "roles",
                Action = "update",
                Description = "Permite atualizar roles"
            },
            new()
            {
                Name = "Delete Roles", Resource = "roles", Action = "delete", Description = "Permite deletar roles"
            },
            new()
            {
                Name = "Assign Permissions",
                Resource = "roles",
                Action = "assign_permissions",
                Description = "Permite atribuir permissões a roles"
            },

            // Permission Management
            new()
            {
                Name = "Read Permissions",
                Resource = "permissions",
                Action = "read",
                Description = "Permite visualizar permissões"
            },

            // Reports
            new()
            {
                Name = "View Reports",
                Resource = "reports",
                Action = "read",
                Description = "Permite visualizar relatórios"
            },
            new()
            {
                Name = "Export Reports",
                Resource = "reports",
                Action = "export",
                Description = "Permite exportar relatórios"
            },

            // System Administration
            new()
            {
                Name = "System Configuration",
                Resource = "system",
                Action = "configure",
                Description = "Permite configurar o sistema"
            },
            new()
            {
                Name = "View Audit Logs",
                Resource = "system",
                Action = "audit",
                Description = "Permite visualizar logs de auditoria"
            },

            // Service Order Management
            new()
            {
                Name = "Create Service Orders",
                Resource = "service_orders",
                Action = "create",
                Description = "Permite criar ordens de serviço"
            },
            new()
            {
                Name = "Read Service Orders",
                Resource = "service_orders",
                Action = "read",
                Description = "Permite visualizar ordens de serviço"
            },
            new()
            {
                Name = "Update Service Orders",
                Resource = "service_orders",
                Action = "update",
                Description = "Permite atualizar ordens de serviço"
            },
            new()
            {
                Name = "Delete Service Orders",
                Resource = "service_orders",
                Action = "delete",
                Description = "Permite deletar ordens de serviço"
            },
            new()
            {
                Name = "Assign Service Orders",
                Resource = "service_orders",
                Action = "assign",
                Description = "Permite atribuir técnico a ordens de serviço"
            },

            // Device Management
            new()
            {
                Name = "Create Devices",
                Resource = "devices",
                Action = "create",
                Description = "Permite cadastrar aparelhos"
            },
            new()
            {
                Name = "Read Devices",
                Resource = "devices",
                Action = "read",
                Description = "Permite visualizar aparelhos"
            },
            new()
            {
                Name = "Update Devices",
                Resource = "devices",
                Action = "update",
                Description = "Permite atualizar informações de aparelhos"
            },
            new()
            {
                Name = "Delete Devices",
                Resource = "devices",
                Action = "delete",
                Description = "Permite deletar aparelhos"
            },

            // Dashboard
            new()
            {
                Name = "View Dashboard",
                Resource = "dashboard",
                Action = "read",
                Description = "Permite visualizar o dashboard"
            }
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        Log.Information("✅ Created {Count} permissions", permissions.Count);
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
