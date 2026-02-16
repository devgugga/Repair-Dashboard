using Microsoft.AspNetCore.Authorization;

namespace Server.Api.Attributes;

/// <summary>
///     Authorization attribute that requires a specific resource-action permission policy.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequirePermissionAttribute" /> class.
    /// </summary>
    /// <param name="resource">The protected resource name.</param>
    /// <param name="action">The required action for the resource.</param>
    public RequirePermissionAttribute(string resource, string action)
    {
        Resource = resource;
        Action = action;
        Policy = $"Permission.{resource}.{action}";
    }

    /// <summary>
    ///     Gets the protected resource name.
    /// </summary>
    public string Resource { get; }

    /// <summary>
    ///     Gets the required action for the protected resource.
    /// </summary>
    public string Action { get; }
}
