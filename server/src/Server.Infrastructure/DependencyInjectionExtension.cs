using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Server.Domain.Interfaces.Repositories;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Domain.Interfaces.Services.Common;
using Server.Domain.Interfaces.Services.Core;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Options.Security;
using Server.Infrastructure.Data;
using Server.Infrastructure.Data.Seeding;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Repositories.Core;
using Server.Infrastructure.Services.Common;
using Server.Infrastructure.Services.Core;
using Server.Infrastructure.Services.Security;

namespace Server.Infrastructure;

/// <summary>
///     Dependency injection registration for infrastructure layer services.
/// </summary>
public static class DependencyInjectionExtension
{
    /// <summary>
    ///     Registers infrastructure repositories, services, options, DbContext, and seeders.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddOptionsValidation(services, configuration);
        AddServices(services, configuration);
        AddDbContext(services, configuration);
        AddDatabaseSeeder(services);
    }

    /// <summary>
    ///     Registers repository implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    }

    /// <summary>
    ///     Registers options binding and startup validation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    private static void AddOptionsValidation(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PasswordOptions>()
            .BindConfiguration(PasswordOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    /// <summary>
    ///     Registers infrastructure services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Security options.
        services.Configure<PasswordOptions>(
            configuration.GetSection(PasswordOptions.SectionName));
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        // Common services.
        services.AddSingleton<ITraceService, TraceService>();

        // Core domain services.
        services.AddScoped<IRbacService, RbacService>();
        services.AddScoped<IAuthService, AuthService>();

        // Security services.
        services.AddScoped<IPasswordHashService, Argon2PasswordHashService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
    }

    /// <summary>
    ///     Registers the application DbContext and provider settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ServerDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)
            );

            // Additional diagnostics for development.
            if (!configuration.GetValue<bool>("IsDevelopment")) return;

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }

    /// <summary>
    ///     Registers database seeding coordinator services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddDatabaseSeeder(IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();
    }
}
