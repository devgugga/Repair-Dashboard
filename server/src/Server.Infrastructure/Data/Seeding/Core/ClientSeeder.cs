using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;
using Server.Domain.Enums.Core;

namespace Server.Infrastructure.Data.Seeding.Core;

public class ClientSeeder : BaseDataSeeder
{
    public override int Order => 2;
    public override string Name => "Client Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.Clients.AnyAsync();
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        // =============================================================================
        // PESSOA FÍSICA - JOÃO SILVA (CPF)
        // =============================================================================
        var joaoPersonId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var joaoClientId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var joaoPerson = new Person
        {
            Id = joaoPersonId,
            Name = "João Silva",
            Document = "12345678901",
            DocumentType = DocumentType.Cpf,
            BirthDate = new DateTimeOffset(1985, 6, 15, 0, 0, 0, TimeSpan.Zero),
            Email = "joao.silva@email.com",
            Phone = "+55 64 9 8888-0001",
            Country = "Brasil",
            State = "Goiás",
            City = "Caldas Novas",
            ZipCode = "75690-000",
            Address = "Rua das Flores",
            AddressNumber = "250",
            AddressComplement = "Casa",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Persons.AddAsync(joaoPerson);
        Log.Information("✅ Created person: {PersonName} (CPF: {Document})", joaoPerson.Name, joaoPerson.Document);

        var joaoClient = new Client
        {
            Id = joaoClientId,
            PersonId = joaoPersonId,
            RepairedAmount = 0,
            Notes = "Cliente residencial, prefere contato por WhatsApp.",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Clients.AddAsync(joaoClient);
        Log.Information("✅ Created client for: {PersonName}", joaoPerson.Name);

        // =============================================================================
        // PESSOA JURÍDICA - TECH SOLUTIONS LTDA (CNPJ)
        // =============================================================================
        var techPersonId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var techClientId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var techPerson = new Person
        {
            Id = techPersonId,
            Name = "Tech Solutions LTDA",
            Document = "12345678000199",
            DocumentType = DocumentType.Cnpj,
            Email = "contato@techsolutions.com.br",
            Phone = "+55 64 3453-0001",
            Country = "Brasil",
            State = "Goiás",
            City = "Caldas Novas",
            ZipCode = "75690-000",
            Address = "Avenida Comercial",
            AddressNumber = "1500",
            AddressComplement = "Sala 302",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Persons.AddAsync(techPerson);
        Log.Information("✅ Created person: {PersonName} (CNPJ: {Document})", techPerson.Name, techPerson.Document);

        var techClient = new Client
        {
            Id = techClientId,
            PersonId = techPersonId,
            RepairedAmount = 0,
            Notes = "Empresa de tecnologia, contrato corporativo. Contato: Maria (Gerente de TI).",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Clients.AddAsync(techClient);
        Log.Information("✅ Created client for: {PersonName}", techPerson.Name);

        // =============================================================================
        // SALVAR ALTERAÇÕES
        // =============================================================================
        await context.SaveChangesAsync();
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
