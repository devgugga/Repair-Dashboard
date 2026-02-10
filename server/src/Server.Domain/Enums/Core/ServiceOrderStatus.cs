namespace Server.Domain.Enums.Core;

public enum ServiceOrderStatus
{
    Pending,
    Diagnosing,
    WaitingApproval,
    WaitingParts,
    InRepair,
    Testing,
    Ready,
    Delivered,
    Cancelled
}
