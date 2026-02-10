using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Server.Domain.Interfaces.Repositories;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Security;
using Server.Infrastructure.Data;
using Server.Infrastructure.Data.Seeding;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Services.Security;

namespace Server.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddOptionsValidation(services, configuration);
        AddServices(services, configuration);
        AddDbContext(services, configuration);
        AddDatabaseSeeder(services);
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }

    private static void AddOptionsValidation(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PasswordOptions>()
            .BindConfiguration(PasswordOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configurações de segurança
        services.Configure<PasswordOptions>(
            configuration.GetSection(PasswordOptions.SectionName));

        // Serviços de segurança
        services.AddScoped<IPasswordHashService, Argon2PasswordHashService>();
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ServerDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)
            );

            // Configurações adicionais para desenvolvimento
            if (!configuration.GetValue<bool>("IsDevelopment")) return;

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }

    private static void AddDatabaseSeeder(IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();
    }
}
