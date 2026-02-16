using System.Net;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Server.Application.DTOs.Response.Common;
using Server.Domain.Exceptions;
using Server.Domain.Exceptions.NotFound;
using Server.Domain.Exceptions.Security;
using Server.Domain.Exceptions.Validation;
using Server.Domain.Interfaces.Services.Common;

namespace Server.Api.Filters;

/// <summary>
///     Global MVC exception filter that maps domain and validation exceptions to problem+json responses.
/// </summary>
public class GlobalExceptionFilter(
    ILogger<GlobalExceptionFilter> logger,
    ITraceService traceService,
    IWebHostEnvironment environment)
    : IExceptionFilter
{
    /// <summary>
    ///     Handles an exception raised during MVC request execution.
    /// </summary>
    /// <param name="context">The exception context.</param>
    public void OnException(ExceptionContext context)
    {
        string traceId = traceService.GetCurrentTraceId();
        string instancePath = context.HttpContext.Request.Path;

        // Log the exception with structured logging.
        logger.LogError(context.Exception,
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
            traceId,
            context.HttpContext.Request.Path,
            context.HttpContext.Request.Method);

        ErrorResponse response = context.Exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, traceId, instancePath),
            DomainValidationException domainValidationEx => CreateDomainValidationErrorResponse(domainValidationEx,
                traceId, instancePath),
            AuthenticationException authEx => CreateAuthenticationErrorResponse(authEx, traceId, instancePath),
            NotFoundException notFoundEx => CreateNotFoundErrorResponse(notFoundEx, traceId, instancePath),
            BusinessRuleViolationException businessEx => CreateBusinessRuleErrorResponse(businessEx, traceId,
                instancePath),
            DomainException domainEx => CreateDomainErrorResponse(domainEx, traceId, instancePath),
            _ => CreateInternalServerErrorResponse(context.Exception, traceId, instancePath)
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.Status, ContentTypes = { "application/problem+json" }
        };

        context.ExceptionHandled = true;
    }

    /// <summary>
    ///     Creates a validation error response for FluentValidation exceptions.
    /// </summary>
    /// <param name="ex">The validation exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateValidationErrorResponse(ValidationException ex, string traceId,
        string instancePath)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var extensions = new Dictionary<string, object> { { "errorCode", "VALIDATION_FAILED" }, { "errors", errors } };

        // Add validation context if available.
        if (ex.Errors.Any())
        {
            string[] errorCodes = ex.Errors
                .Where(e => !string.IsNullOrEmpty(e.ErrorCode))
                .Select(e => e.ErrorCode)
                .Distinct()
                .ToArray();

            if (errorCodes.Length > 0)
            {
                extensions["errorCodes"] = errorCodes;
                extensions["validationContext"] = "FluentValidation";
            }
        }

        return new ErrorResponse
        {
            Type = ErrorTypes.Validation,
            Title = ErrorTitles.ValidationFailed,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "The request contains invalid data.",
            Instance = instancePath,
            TraceId = traceId,
            Extensions = extensions
        };
    }

    /// <summary>
    ///     Creates a validation error response for domain validation exceptions.
    /// </summary>
    /// <param name="ex">The domain validation exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateDomainValidationErrorResponse(DomainValidationException ex, string traceId,
        string instancePath)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.Validation,
            Title = ErrorTitles.ValidationFailed,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = ex.Message,
            Instance = instancePath,
            TraceId = traceId,
            Extensions = new Dictionary<string, object>
            {
                { "errorCode", ex.ErrorCode }, { "errors", ex.ValidationErrors }
            }
        };
    }

    /// <summary>
    ///     Creates an authentication error response.
    /// </summary>
    /// <param name="ex">The authentication exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateAuthenticationErrorResponse(AuthenticationException ex, string traceId,
        string instancePath)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.Authentication,
            Title = ErrorTitles.AuthenticationFailed,
            Status = (int)HttpStatusCode.Unauthorized,
            Detail = ex.Message,
            Instance = instancePath,
            TraceId = traceId,
            Extensions = new Dictionary<string, object> { { "errorCode", ex.ErrorCode } }
        };
    }

    /// <summary>
    ///     Creates a not-found error response.
    /// </summary>
    /// <param name="ex">The not-found exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateNotFoundErrorResponse(NotFoundException ex, string traceId,
        string instancePath)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.NotFound,
            Title = ErrorTitles.ResourceNotFound,
            Status = (int)HttpStatusCode.NotFound,
            Detail = ex.Message,
            Instance = instancePath,
            TraceId = traceId,
            Extensions = new Dictionary<string, object> { { "errorCode", ex.ErrorCode } }
        };
    }

    /// <summary>
    ///     Creates a conflict error response for business rule violations.
    /// </summary>
    /// <param name="ex">The business rule violation exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateBusinessRuleErrorResponse(BusinessRuleViolationException ex, string traceId,
        string instancePath)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.Conflict,
            Title = ErrorTitles.ConflictError,
            Status = (int)HttpStatusCode.Conflict,
            Detail = ex.Message,
            Instance = instancePath,
            TraceId = traceId,
            Extensions = new Dictionary<string, object> { { "errorCode", ex.ErrorCode } }
        };
    }

    /// <summary>
    ///     Creates a bad-request error response for generic domain exceptions.
    /// </summary>
    /// <param name="ex">The domain exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private static ErrorResponse CreateDomainErrorResponse(DomainException ex, string traceId, string instancePath)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.BadRequest,
            Title = ErrorTitles.BadRequest,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = ex.Message,
            Instance = instancePath,
            TraceId = traceId,
            Extensions = new Dictionary<string, object> { { "errorCode", ex.ErrorCode } }
        };
    }

    /// <summary>
    ///     Creates an internal-server-error response for unknown exceptions.
    /// </summary>
    /// <param name="ex">The unhandled exception.</param>
    /// <param name="traceId">The correlation trace id.</param>
    /// <param name="instancePath">The request path.</param>
    /// <returns>An error response payload.</returns>
    private ErrorResponse CreateInternalServerErrorResponse(Exception ex, string traceId, string instancePath)
    {
        string detail = environment.IsDevelopment()
            ? ex.Message
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
                { "stackTrace", ex.StackTrace ?? string.Empty }, { "exceptionType", ex.GetType().Name }
            };

        return response;
    }
}
