using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Enums.Core;

namespace Server.Domain.Entities.Core;

[Table("ServiceOrders")]
public class ServiceOrder : BaseEntity
{
    public string Code { get; set; }

    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;

    public ServiceOrderPriority Priority { get; set; } = ServiceOrderPriority.Normal;

    public DateTimeOffset EntryDate { get; set; }

    public DateTimeOffset? EstimatedCompletionDate { get; set; }

    public DateTimeOffset? CompletionDate { get; set; }

    public string DefectDescription { get; set; }

    public string? TechnicianDiagnosis { get; set; }

    public string? InternalNotes { get; set; }

    public decimal? EstimatedCost { get; set; }

    public decimal? FinalCost { get; set; }

    public Guid ClientId { get; set; }
    public virtual Client Client { get; set; } = null!;

    public Guid DeviceId { get; set; }
    public virtual Device Device { get; set; } = null!;

    public Guid? TechnicianId { get; set; }
    public virtual User? Technician { get; set; }

    public virtual ICollection<ServiceOrderItem> Items { get; set; } = [];
}
