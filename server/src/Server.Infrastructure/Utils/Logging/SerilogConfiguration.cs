using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Server.Infrastructure.Utils.Logging;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        ConfigurationManager configuration = builder.Configuration;
        IWebHostEnvironment environment = builder.Environment;

        // Criar diretório de logs se não existir
        string logPath = configuration["Logging:Serilog:LogPath"] ?? "logs";
        Directory.CreateDirectory(logPath);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithExceptionDetails()
            .Enrich.With<RequestEnricher>()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} | {Message:lj} {NewLine}{Exception}",
                restrictedToMinimumLevel: environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information
            )
            .WriteTo.File(
                Path.Combine(logPath, "server-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} | {Message:lj} | {Properties} {NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.File(
                Path.Combine(logPath, "server-errors-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90,
                restrictedToMinimumLevel: LogEventLevel.Warning,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} | {Message:lj} | {Properties} {NewLine}{Exception}"
            )
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.WithProperty("EventType", "SecurityAlert"))
                .WriteTo.File(
                    Path.Combine(logPath, "security", "security-alerts-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 365,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | {Message:lj} | {Properties} {NewLine}"
                )
            )
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.WithProperty("EventType", "Audit"))
                .WriteTo.File(
                    Path.Combine(logPath, "audit", "audit-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 365,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | {Message:lj} | {Properties} {NewLine}"
                )
            )
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.WithProperty("EventType", "Performance"))
                .WriteTo.File(
                    Path.Combine(logPath, "performance", "performance-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Message:lj} | {Properties} {NewLine}"
                )
            )
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting"))
            .Filter.ByExcluding(Matching.FromSource("Microsoft.Extensions.Hosting"))
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Debug()
            .CreateLogger();

        builder.Host.UseSerilog();
    }

}
