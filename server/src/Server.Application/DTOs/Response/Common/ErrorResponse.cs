namespace Server.Application.DTOs.Response.Common;

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Extensions { get; set; }
}

public static class ErrorTypes
{
    public const string Validation = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    public const string Authentication = "https://tools.ietf.org/html/rfc7235#section-3.1";
    public const string Authorization = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
    public const string NotFound = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
    public const string Conflict = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
    public const string InternalServerError = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
    public const string BadRequest = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
}

public static class ErrorTitles
{
    public const string ValidationFailed = "One or more validation errors occurred.";
    public const string AuthenticationFailed = "Authentication failed.";
    public const string AccessDenied = "Access denied.";
    public const string ResourceNotFound = "The specified resource was not found.";
    public const string ConflictError = "A conflict occurred while processing the request.";
    public const string InternalServerError = "An error occurred while processing your request.";
    public const string BadRequest = "The request is invalid.";
}
