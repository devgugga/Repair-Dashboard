using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using Server.Application.DTOs.Request.Core;
using Server.Application.Mapping.Core;
using Server.Application.UseCases.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Application.Validators.Core;

namespace Server.Application;

/// <summary>
///     Dependency injection registration for application layer services.
/// </summary>
public static class DependencyInjectionExtension
{
    /// <summary>
    ///     Registers application use cases, mappers, and validators.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddUseCases();
        services.AddMappers();
        services.AddValidators();
    }

    /// <summary>
    ///     Registers use case implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IAuthUseCase, AuthUseCase>();
        services.AddScoped<IRoleUseCase, RoleUseCase>();
        services.AddScoped<IPermissionUseCase, PermissionUseCase>();
        services.AddScoped<IUserRoleUseCase, UserRoleUseCase>();
    }

    /// <summary>
    ///     Registers AutoMapper profiles used by the application layer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AuthMappingProfile>();
            cfg.AddProfile<RbacMappingProfile>();
        });
    }

    /// <summary>
    ///     Registers FluentValidation validators used by the application layer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();
    }
}
