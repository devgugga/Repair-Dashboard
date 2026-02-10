using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Serilog.Core;
using Serilog.Events;

namespace Server.Infrastructure.Utils.Logging;

public class RequestEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        HttpContext? httpContext = GetHttpContext();
        if (httpContext == null) return;

        // Adicionar IP do cliente
        string clientIp = GetClientIpAddress(httpContext);
        if (!string.IsNullOrEmpty(clientIp))
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ClientIP", clientIp));

        // Adicionar User Agent
        string? userAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault();
        if (!string.IsNullOrEmpty(userAgent))
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UserAgent", userAgent));

        // Adicionar método HTTP e URL
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("HttpMethod", httpContext.Request.Method));
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path.Value));

        // Adicionar correlation ID se existir
        string correlationId = httpContext.TraceIdentifier;
        if (!string.IsNullOrEmpty(correlationId))
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CorrelationId", correlationId));
    }

    private static HttpContext? GetHttpContext()
    {
        IHttpContextAccessor? httpContextAccessor = GetService<IHttpContextAccessor>();
        return httpContextAccessor?.HttpContext;
    }

    private static T? GetService<T>() where T : class
    {
        try
        {
            IServiceProvider? serviceProvider = GetServiceProvider();
            return serviceProvider?.GetService<T>();
        }
        catch
        {
            return null;
        }
    }

    private static IServiceProvider? GetServiceProvider()
    {
        // Método para obter o service provider atual
        // Isso será configurado no startup
        return ServiceProviderAccessor.ServiceProvider;
    }

    private static string GetClientIpAddress(HttpContext httpContext)
    {
        string? ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress)) return ipAddress.Split(',')[0].Trim();

        ipAddress = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress)) return ipAddress;

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

public static class ServiceProviderAccessor
{
    public static IServiceProvider? ServiceProvider { get; set; }
}
