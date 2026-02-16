using System.Net;
using System.Text.Json;

using Server.Application.DTOs.Response.Common;
using Server.Domain.Interfaces.Services.Common;

namespace Server.Api.Middlewares;

/// <summary>
///     Middleware that handles unhandled exceptions and returns problem+json responses.
/// </summary>
public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    ITraceService traceService,
    IWebHostEnvironment environment)
{
    /// <summary>
    ///     Invokes the middleware for the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous middleware execution.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Ensure trace id is initialized for this request.
            EnsureTraceId(context);

            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    ///     Ensures trace metadata is present for correlation.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    private void EnsureTraceId(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.TraceIdentifier)) context.TraceIdentifier = traceService.GenerateTraceId();

        // Add trace id to response headers for client tracking.
        context.Response.Headers.TryAdd("X-Trace-Id", context.TraceIdentifier);
    }

    /// <summary>
    ///     Handles an exception and writes a standardized error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string traceId = traceService.GetCurrentTraceId();
        string instancePath = context.Request.Path;

        logger.LogError(exception,
            "Unhandled exception in middleware. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
            traceId,
            context.Request.Path,
            context.Request.Method);

        ErrorResponse response = CreateErrorResponse(exception, traceId, instancePath);

        context.Response.StatusCode = response.Status;
        context.Response.ContentType = "application/problem+json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = environment.IsDevelopment()
        };

        string jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    ///     Creates an error response model from an exception.
    /// </summary>
    /// <param name="exception">The source exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>A standardized error response payload.</returns>
    private ErrorResponse CreateErrorResponse(Exception exception, string traceId, string instancePath)
    {
        string detail = environment.IsDevelopment()
            ? exception.Message
            : "An unexpected error occurred.";

        var response = new ErrorResponse
        {
            Type = ErrorTypes.InternalServerError,
            Title = ErrorTitles.InternalServerError,
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = detail,
            Instance = instancePath,
            TraceId = traceId
        };

        if (environment.IsDevelopment())
            response.Extensions = new Dictionary<string, object>
            {
                { "stackTrace", exception.StackTrace ?? string.Empty },
                { "exceptionType", exception.GetType().Name }
            };

        return response;
    }
}
