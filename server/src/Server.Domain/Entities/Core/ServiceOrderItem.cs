using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Enums.Core;

namespace Server.Domain.Entities.Core;

[Table("ServiceOrderItems")]
public class ServiceOrderItem : BaseEntity
{
    public string Description { get; set; }

    public ServiceOrderItemType Type { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Guid ServiceOrderId { get; set; }
    public virtual ServiceOrder ServiceOrder { get; set; } = null!;
}
