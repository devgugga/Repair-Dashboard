using System.Diagnostics;

using Microsoft.AspNetCore.Http;

using Server.Domain.Interfaces.Services.Common;

namespace Server.Infrastructure.Services.Common;

public class TraceService(IHttpContextAccessor httpContextAccessor) : ITraceService
{
    public string GetCurrentTraceId()
    {
        // Try to get from HttpContext first (if in web request)
        string? httpTraceId = httpContextAccessor.HttpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(httpTraceId)) return httpTraceId;

        // Fallback to Activity.Current (for background operations)
        string? activityTraceId = Activity.Current?.TraceId.ToString();
        if (!string.IsNullOrEmpty(activityTraceId)) return activityTraceId;

        // Last resort: generate a new one
        return GenerateTraceId();
    }

    public void SetTraceId(string traceId)
    {
        if (httpContextAccessor.HttpContext != null) httpContextAccessor.HttpContext.TraceIdentifier = traceId;

        // Also set in Activity if available
        if (Activity.Current != null) Activity.Current.SetTag("traceId", traceId);
    }

    public string GenerateTraceId()
    {
        return Guid.NewGuid().ToString("N")[..16]; // 16-character hex string
    }
}
