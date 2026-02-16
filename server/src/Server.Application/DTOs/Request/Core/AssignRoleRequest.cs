namespace Server.Application.DTOs.Request.Core;

/// <summary>
///     Request payload used to assign roles to a user.
/// </summary>
public class AssignRoleRequest
{
    /// <summary>
    ///     Gets or sets the role identifiers to assign.
    /// </summary>
    public required List<Guid> RoleIds { get; set; }

    /// <summary>
    ///     Gets or sets an optional assignment expiration date.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}
