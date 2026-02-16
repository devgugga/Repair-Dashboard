using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("Roles")]
public class Role : BaseEntity
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public bool IsSystemRole { get; set; } = false;

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
}
