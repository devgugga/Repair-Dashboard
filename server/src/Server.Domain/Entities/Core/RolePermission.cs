using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("RolePermissions")]
public class RolePermission : BaseEntity
{
    public required Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;

    public required Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;

    public required Guid AssignedBy { get; set; }

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
}
