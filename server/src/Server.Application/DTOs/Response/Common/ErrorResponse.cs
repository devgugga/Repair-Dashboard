namespace Server.Application.DTOs.Response.Common;

/// <summary>
///     Standard RFC7807-style error response payload.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    ///     Gets or sets the error type URI.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the short error title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the HTTP status code.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    ///     Gets or sets the detailed error message.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the request instance path.
    /// </summary>
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the correlation trace identifier.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the timestamp when the error response was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets additional extension fields.
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}

/// <summary>
///     Canonical URI references for known API error categories.
/// </summary>
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

/// <summary>
///     Human-readable default titles for known API error categories.
/// </summary>
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
