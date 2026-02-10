using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Enums.Core;

namespace Server.Domain.Entities.Core;

[Table("Users")]
public class User : BaseEntity
{
    public string UserName { get; set; }

    public string PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.Viewer;

    public DateTimeOffset? LastLogin { get; set; }

    public bool IsLocked { get; set; } = false;

    public uint FailedLoginAttempts { get; set; } = 0;

    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<ServiceOrder> AssignedServiceOrders { get; set; } = [];
}
