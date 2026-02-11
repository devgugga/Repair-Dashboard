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

public class GlobalExceptionFilter(
    ILogger<GlobalExceptionFilter> logger,
    ITraceService traceService,
    IWebHostEnvironment environment)
    : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        string traceId = traceService.GetCurrentTraceId();
        string instancePath = context.HttpContext.Request.Path;

        // Log the exception with structured logging
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

    private static ErrorResponse CreateValidationErrorResponse(ValidationException ex, string traceId,
        string instancePath)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var extensions = new Dictionary<string, object> { { "errorCode", "VALIDATION_FAILED" }, { "errors", errors } };

        // Add validation context if available
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
