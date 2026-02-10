using Microsoft.EntityFrameworkCore;

using Serilog;

using Server.Domain.Entities.Core;
using Server.Domain.Enums.Core;
using Server.Domain.Interfaces.Services.Security;

namespace Server.Infrastructure.Data.Seeding.Core;

public class UserSeeder : BaseDataSeeder
{
    public override int Order => 1;
    public override string Name => "User and Admin Seeder";

    public override async Task<bool> HasBeenSeededAsync(ServerDbContext context)
    {
        return await context.Users.AnyAsync(u => u.UserName == "admin");
    }

    public override async Task SeedAsync(ServerDbContext context, IServiceProvider serviceProvider)
    {
        Log.Information("🌱 Starting {SeederName}...", Name);

        IPasswordHashService passwordService = GetRequiredService<IPasswordHashService>(serviceProvider);

        // =============================================================================
        // PESSOA PARA USUÁRIO ADMIN
        // =============================================================================
        var adminPersonId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var adminPerson = new Person
        {
            Id = adminPersonId,
            Name = "Administrador do Sistema",
            Document = "00000000000",
            DocumentType = DocumentType.Cpf,
            BirthDate = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Email = "admin@repair.com.br",
            Phone = "+55 64 9 9999-0001",
            Country = "Brasil",
            State = "Goiás",
            City = "Caldas Novas",
            ZipCode = "75690-000",
            Address = "Rua Principal do Sistema",
            AddressNumber = "100",
            AddressComplement = "Sala Administrativa",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Persons.AddAsync(adminPerson);
        Log.Information("✅ Created admin person: {PersonName} ({Email})",
            adminPerson.Name, adminPerson.Email);

        // =============================================================================
        // USUÁRIO ADMIN
        // =============================================================================
        var adminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Gerar hash da senha "admin123" usando Argon2id
        string adminPasswordHash = await passwordService.HashPasswordAsync("admin123");

        var adminUser = new User
        {
            Id = adminUserId,
            PersonId = adminPersonId,
            UserName = "admin",
            PasswordHash = adminPasswordHash,
            Role = UserRole.Admin,
            IsLocked = false,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Users.AddAsync(adminUser);
        Log.Information("✅ Created admin user: {UserName} (Password: admin123)",
            adminUser.UserName);

        // =============================================================================
        // SALVAR ALTERAÇÕES
        // =============================================================================
        await context.SaveChangesAsync();
        Log.Information("🎉 {SeederName} completed successfully!", Name);
    }
}
