using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("Clients")]
public class Client : BaseEntity
{
    public uint RepairedAmount { get; set; } = 0;

    public string? Notes { get; set; }

    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = [];
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = [];
}
