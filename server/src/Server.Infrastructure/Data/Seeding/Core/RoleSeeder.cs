using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class RoleSeeder : BaseDataSeeder
{
    public override int Order => 3;
    public override string Name => "Role Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.Roles.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        var systemAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var managerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var technicianRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var viewerRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var clientRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        var roles = new List<Role>
        {
            new()
            {
                Id = systemAdminId,
                Name = "System Administrator",
                Description = "Administrador do sistema com acesso total a todas as funcionalidades",
                IsSystemRole = true
            },
            new()
            {
                Id = managerRoleId,
                Name = "Manager",
                Description = "Gerente da loja com acesso a relatórios, gestão de clientes e ordens de serviço"
            },
            new()
            {
                Id = technicianRoleId,
                Name = "Technician",
                Description = "Técnico de reparos com acesso a ordens de serviço e aparelhos"
            },
            new() { Id = viewerRoleId, Name = "Viewer", Description = "Visualizador com acesso somente leitura" },
            new()
            {
                Id = clientRoleId,
                Name = "Client",
                Description = "Cliente com acesso limitado aos próprios dados"
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        Log.Information("✅ Created {Count} roles", roles.Count);
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
