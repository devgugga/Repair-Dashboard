using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("UserRoles")]
public class UserRole : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public required Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;

    public required Guid AssignedBy { get; set; }

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; set; }
}
