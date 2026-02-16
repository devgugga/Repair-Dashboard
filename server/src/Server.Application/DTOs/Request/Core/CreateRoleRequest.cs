namespace Server.Application.DTOs.Request.Core;

/// <summary>
///     Request payload used to create a new role.
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    ///     Gets or sets the role name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the role description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Gets or sets the permission identifiers assigned to the role.
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = [];
}
