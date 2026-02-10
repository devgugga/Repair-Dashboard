using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;
using Server.Domain.Enums.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class ServiceOrderSeeder : BaseDataSeeder
{
    public override int Order => 4;
    public override string Name => "Service Order Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.ServiceOrders.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        // IDs de entidades criadas pelos seeders anteriores
        var joaoClientId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var techClientId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var iPhoneId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var galaxyId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var macBookId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var adminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // =============================================================================
        // OS-2025-001: IPHONE - TELA QUEBRADA (InRepair, High)
        // =============================================================================
        var os001Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var os001 = new ServiceOrder
        {
            Id = os001Id,
            Code = "OS-2025-001",
            Status = ServiceOrderStatus.InRepair,
            Priority = ServiceOrderPriority.High,
            EntryDate = new DateTimeOffset(2025, 1, 10, 9, 0, 0, TimeSpan.Zero),
            EstimatedCompletionDate = new DateTimeOffset(2025, 1, 15, 18, 0, 0, TimeSpan.Zero),
            DefectDescription =
                "Tela quebrada após queda. Display com linhas verticais e touch não responde na parte inferior.",
            TechnicianDiagnosis = "Display OLED danificado. Necessário troca completa do módulo de tela.",
            InternalNotes = "Cliente tem seguro, verificar cobertura antes de prosseguir.",
            EstimatedCost = 1200.00m,
            ClientId = joaoClientId,
            DeviceId = iPhoneId,
            TechnicianId = adminUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrders.AddAsync(os001);
        Log.Information("✅ Created service order: {Code} ({Status})", os001.Code, os001.Status);

        var os001Item1 = new ServiceOrderItem
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"),
            Description = "Tela OLED iPhone 15 Pro - Original",
            Type = ServiceOrderItemType.Part,
            Quantity = 1,
            UnitPrice = 950.00m,
            ServiceOrderId = os001Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var os001Item2 = new ServiceOrderItem
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02"),
            Description = "Mão de obra - Troca de tela iPhone",
            Type = ServiceOrderItemType.Labor,
            Quantity = 1,
            UnitPrice = 250.00m,
            ServiceOrderId = os001Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrderItems.AddAsync(os001Item1);
        await context.ServiceOrderItems.AddAsync(os001Item2);
        Log.Information("✅ Created {Count} items for {Code}", 2, os001.Code);

        // =============================================================================
        // OS-2025-002: GALAXY - BATERIA (Pending, Normal)
        // =============================================================================
        var os002Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab");

        var os002 = new ServiceOrder
        {
            Id = os002Id,
            Code = "OS-2025-002",
            Status = ServiceOrderStatus.Pending,
            Priority = ServiceOrderPriority.Normal,
            EntryDate = new DateTimeOffset(2025, 1, 12, 14, 30, 0, TimeSpan.Zero),
            EstimatedCompletionDate = new DateTimeOffset(2025, 1, 20, 18, 0, 0, TimeSpan.Zero),
            DefectDescription = "Bateria descarregando muito rápido. Celular desliga com 30% de carga.",
            EstimatedCost = 350.00m,
            ClientId = joaoClientId,
            DeviceId = galaxyId,
            TechnicianId = null,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrders.AddAsync(os002);
        Log.Information("✅ Created service order: {Code} ({Status})", os002.Code, os002.Status);

        var os002Item1 = new ServiceOrderItem
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03"),
            Description = "Bateria Samsung Galaxy S24 - Original",
            Type = ServiceOrderItemType.Part,
            Quantity = 1,
            UnitPrice = 250.00m,
            ServiceOrderId = os002Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrderItems.AddAsync(os002Item1);
        Log.Information("✅ Created {Count} item for {Code}", 1, os002.Code);

        // =============================================================================
        // OS-2025-003: MACBOOK - NÃO LIGA (Diagnosing, Urgent)
        // =============================================================================
        var os003Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac");

        var os003 = new ServiceOrder
        {
            Id = os003Id,
            Code = "OS-2025-003",
            Status = ServiceOrderStatus.Diagnosing,
            Priority = ServiceOrderPriority.Urgent,
            EntryDate = new DateTimeOffset(2025, 1, 14, 8, 0, 0, TimeSpan.Zero),
            EstimatedCompletionDate = new DateTimeOffset(2025, 1, 21, 18, 0, 0, TimeSpan.Zero),
            DefectDescription =
                "MacBook não liga. Sem sinais de vida ao pressionar botão de energia. Sem danos físicos visíveis.",
            TechnicianDiagnosis =
                "Diagnóstico em andamento. Possível problema na placa lógica ou fonte de alimentação.",
            InternalNotes = "Equipamento corporativo, prioridade alta do cliente.",
            ClientId = techClientId,
            DeviceId = macBookId,
            TechnicianId = adminUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrders.AddAsync(os003);
        Log.Information("✅ Created service order: {Code} ({Status})", os003.Code, os003.Status);

        var os003Item1 = new ServiceOrderItem
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04"),
            Description = "Diagnóstico avançado - Placa lógica MacBook Pro",
            Type = ServiceOrderItemType.Labor,
            Quantity = 1,
            UnitPrice = 200.00m,
            ServiceOrderId = os003Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.ServiceOrderItems.AddAsync(os003Item1);
        Log.Information("✅ Created {Count} item for {Code}", 1, os003.Code);

        // =============================================================================
        // SALVAR ALTERAÇÕES
        // =============================================================================
        await context.SaveChangesAsync();
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
