namespace Server.Domain.Interfaces.Services.Common;

/// <summary>
///     Service responsible for managing trace identifiers used for request correlation and distributed tracing.
/// </summary>
public interface ITraceService
{
    /// <summary>
    ///     Gets the trace identifier for the current request context.
    /// </summary>
    /// <returns>The current trace identifier.</returns>
    string GetCurrentTraceId();

    /// <summary>
    ///     Sets the trace identifier for the current request context.
    /// </summary>
    /// <param name="traceId">The trace identifier to set.</param>
    void SetTraceId(string traceId);

    /// <summary>
    ///     Generates a new unique trace identifier.
    /// </summary>
    /// <returns>A newly generated trace identifier.</returns>
    string GenerateTraceId();
}
