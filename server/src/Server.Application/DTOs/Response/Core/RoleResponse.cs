namespace Server.Application.DTOs.Response.Core;

/// <summary>
///     Role response payload.
/// </summary>
public class RoleResponse
{
    /// <summary>
    ///     Gets or sets the role identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the role name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the role description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the role is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    ///     Gets or sets whether the role is system-defined.
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    ///     Gets or sets permissions associated with the role.
    /// </summary>
    public List<PermissionResponse> Permissions { get; set; } = [];

    /// <summary>
    ///     Gets or sets the role creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
