using System.Diagnostics;

namespace Server.Api.Middlewares;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        string correlationId = context.TraceIdentifier;

        logger.LogInformation("Starting request {Method} {Path}", context.Request.Method, context.Request.Path);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Method} {Path} failed with exception", context.Request.Method,
                context.Request.Path);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (context.Response.StatusCode >= 400)
                logger.LogWarning("Completed request {Method} {Path} with status {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            else
                logger.LogInformation("Completed request {Method} {Path} with status {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
        }
    }
}
