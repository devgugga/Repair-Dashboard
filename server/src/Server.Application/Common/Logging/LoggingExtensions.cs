using Serilog;

namespace Server.Application.Common.Logging;

public static class LoggingExtensions
{
    // Security Alert Logging
    public static void LogSecurityAlert(this ILogger logger, string alertType, string description,
        object? additionalData = null)
    {
        logger.ForContext("EventType", "SecurityAlert")
            .ForContext("AlertType", alertType)
            .ForContext("AdditionalData", additionalData, true)
            .Warning("Security Alert: {AlertType} - {Description}", alertType, description);
    }

    // Performance Logging
    public static void LogPerformance(this ILogger logger, string operation, long elapsedMs, object? metadata = null)
    {
        logger.ForContext("EventType", "Performance")
            .ForContext("Operation", operation)
            .ForContext("ElapsedMs", elapsedMs)
            .ForContext("Metadata", metadata, true)
            .Information("Performance: {Operation} completed in {ElapsedMs}ms", operation, elapsedMs);
    }


    // Audit Logging
    public static void LogAudit(this ILogger logger, string operation, string entityId, string entityType,
        object? details = null)
    {
        logger.ForContext("EventType", "Audit")
            .ForContext("Operation", operation)
            .ForContext("EntityId", entityId)
            .ForContext("EntityType", entityType)
            .ForContext("Details", details, true)
            .Information("Audit: {Operation} on {EntityType} {EntityId}", operation, entityType, entityId);
    }

}
