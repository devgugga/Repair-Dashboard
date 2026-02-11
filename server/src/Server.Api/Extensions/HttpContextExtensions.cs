namespace Server.Api.Extensions;

/// <summary>
///     Extension methods for <see cref="HttpContext" /> providing common HTTP utilities.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    ///     Gets the client IP address from the request, checking proxy headers first.
    ///     Checks X-Forwarded-For, then X-Real-IP, then RemoteIpAddress.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The client IP address or "Unknown" if not available.</returns>
    public static string GetClientIpAddress(this HttpContext context)
    {
        // Check for forwarded IP first (behind proxy/load balancer)
        string? forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor)) return forwardedFor.Split(',')[0].Trim();

        string? realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp)) return realIp;

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
