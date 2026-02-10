using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class DeviceSeeder : BaseDataSeeder
{
    public override int Order => 3;
    public override string Name => "Device Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.Devices.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        // IDs dos clientes criados pelo ClientSeeder
        var joaoClientId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var techClientId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        // =============================================================================
        // IPHONE 15 PRO - CLIENTE JOÃO
        // =============================================================================
        var iPhoneId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        var iPhone = new Device
        {
            Id = iPhoneId,
            Brand = "Apple",
            Model = "iPhone 15 Pro",
            Color = "Titânio Natural",
            SerialNumber = "F2LXF1GHMU",
            Imei = "354833116078520",
            Condition = "Tela trincada, funcionamento normal",
            ClientId = joaoClientId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Devices.AddAsync(iPhone);
        Log.Information("✅ Created device: {Brand} {Model} (Client: João Silva)", iPhone.Brand, iPhone.Model);

        // =============================================================================
        // SAMSUNG GALAXY S24 - CLIENTE JOÃO
        // =============================================================================
        var galaxyId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        var galaxy = new Device
        {
            Id = galaxyId,
            Brand = "Samsung",
            Model = "Galaxy S24",
            Color = "Preto",
            Imei = "356938035643809",
            Condition = "Bateria viciada, descarrega rápido",
            ClientId = joaoClientId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Devices.AddAsync(galaxy);
        Log.Information("✅ Created device: {Brand} {Model} (Client: João Silva)", galaxy.Brand, galaxy.Model);

        // =============================================================================
        // MACBOOK PRO 14" - CLIENTE TECH SOLUTIONS
        // =============================================================================
        var macBookId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var macBook = new Device
        {
            Id = macBookId,
            Brand = "Apple",
            Model = "MacBook Pro 14\"",
            Color = "Cinza Espacial",
            SerialNumber = "C02ZW1XYMD6T",
            Condition = "Não liga, sem sinais de dano físico",
            ClientId = techClientId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Devices.AddAsync(macBook);
        Log.Information("✅ Created device: {Brand} {Model} (Client: Tech Solutions LTDA)", macBook.Brand,
            macBook.Model);

        // =============================================================================
        // SALVAR ALTERAÇÕES
        // =============================================================================
        await context.SaveChangesAsync();
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
