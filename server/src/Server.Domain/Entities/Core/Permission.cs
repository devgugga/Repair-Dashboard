using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities.Core;

[Table("Permissions")]
public class Permission : BaseEntity
{
    public required string Name { get; set; }

    public required string Resource { get; set; }

    public required string Action { get; set; }

    public required string Description { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];

    public string FullPermission => $"{Resource}.{Action}";
}
