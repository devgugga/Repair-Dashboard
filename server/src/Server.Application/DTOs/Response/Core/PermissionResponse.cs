namespace Server.Application.DTOs.Response.Core;

/// <summary>
///     Permission response payload.
/// </summary>
public class PermissionResponse
{
    /// <summary>
    ///     Gets or sets the permission identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the permission display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the protected resource name.
    /// </summary>
    public required string Resource { get; set; }

    /// <summary>
    ///     Gets or sets the allowed action name.
    /// </summary>
    public required string Action { get; set; }

    /// <summary>
    ///     Gets or sets the permission description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    ///     Gets or sets the composed permission key in the format resource.action.
    /// </summary>
    public string FullPermission { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets whether the permission is active.
    /// </summary>
    public bool IsActive { get; set; }
}
