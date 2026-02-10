using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("Devices")]
public class Device : BaseEntity
{
    public string Brand { get; set; }

    public string Model { get; set; }

    public string? Color { get; set; }

    public string? SerialNumber { get; set; }

    public string? Imei { get; set; }

    public string? Condition { get; set; }

    public Guid ClientId { get; set; }
    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = [];
}
