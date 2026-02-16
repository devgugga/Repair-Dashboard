using Microsoft.AspNetCore.Authorization;

namespace Server.Api.Authorization;

/// <summary>
///     Authorization requirement that represents a resource-action permission.
/// </summary>
public class PermissionRequirement(string resource, string action) : IAuthorizationRequirement
{
    /// <summary>
    ///     Gets the required resource name.
    /// </summary>
    public string Resource { get; } = resource;

    /// <summary>
    ///     Gets the required action name.
    /// </summary>
    public string Action { get; } = action;
}
