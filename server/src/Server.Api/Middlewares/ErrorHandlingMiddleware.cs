using System.Net;
using System.Text.Json;

using Server.Application.DTOs.Response.Common;
using Server.Domain.Interfaces.Services.Common;

namespace Server.Api.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    ITraceService traceService,
    IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Ensure TraceId is set for the request
            EnsureTraceId(context);

            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private void EnsureTraceId(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.TraceIdentifier)) context.TraceIdentifier = traceService.GenerateTraceId();

        // Add TraceId to response headers for client tracking
        context.Response.Headers.TryAdd("X-Trace-Id", context.TraceIdentifier);
    }

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
