using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using Server.Application.DTOs.Request.Core;
using Server.Application.Mapping.Core;
using Server.Application.UseCases.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Application.Validators.Core;

namespace Server.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddUseCases();
        services.AddMappers();
        services.AddValidators();
    }

    private static void AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IAuthUseCase, AuthUseCase>();
        services.AddScoped<IRoleUseCase, RoleUseCase>();
        services.AddScoped<IPermissionUseCase, PermissionUseCase>();
        services.AddScoped<IUserRoleUseCase, UserRoleUseCase>();

    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AuthMappingProfile>();
            cfg.AddProfile<RbacMappingProfile>();
        });
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();
    }
}
