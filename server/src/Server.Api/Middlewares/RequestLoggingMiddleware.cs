using System.Diagnostics;

using Server.Api.Extensions;
using Server.Domain.Interfaces.Services.Common;

namespace Server.Api.Middlewares;

/// <summary>
///     Middleware that logs request start/end information with correlation metadata.
/// </summary>
public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger,
    ITraceService traceService)
{
    /// <summary>
    ///     Invokes the middleware for the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous middleware execution.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        string traceId = traceService.GetCurrentTraceId();

        // Extract additional request information.
        string userAgent = context.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
        string clientIp = context.GetClientIpAddress();
        string userId = context.User?.Identity?.Name ?? "Anonymous";

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["TraceId"] = traceId, ["UserId"] = userId, ["ClientIp"] = clientIp, ["UserAgent"] = userAgent
               }))
        {
            logger.LogInformation("Starting request {Method} {Path} {QueryString}",
                context.Request.Method, context.Request.Path, context.Request.QueryString);

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Request {Method} {Path} failed with exception",
                    context.Request.Method, context.Request.Path);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                LogLevel logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error :
                    context.Response.StatusCode >= 400 ? LogLevel.Warning :
                    LogLevel.Information;

                logger.Log(logLevel,
                    "Completed request {Method} {Path} with status {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
