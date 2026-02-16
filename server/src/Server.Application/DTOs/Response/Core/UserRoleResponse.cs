namespace Server.Application.DTOs.Response.Core;

/// <summary>
///     Response payload containing a user's assigned roles and effective permissions.
/// </summary>
public class UserRoleResponse
{
    /// <summary>
    ///     Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    ///     Gets or sets the user e-mail address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    ///     Gets or sets the assigned roles.
    /// </summary>
    public List<RoleResponse> Roles { get; set; } = [];

    /// <summary>
    ///     Gets or sets the effective permissions.
    /// </summary>
    public List<PermissionResponse> Permissions { get; set; } = [];
}
