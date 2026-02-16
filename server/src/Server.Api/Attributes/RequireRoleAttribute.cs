using Microsoft.AspNetCore.Authorization;

namespace Server.Api.Attributes;

/// <summary>
///     Authorization attribute that requires one or more roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : AuthorizeAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequireRoleAttribute" /> class.
    /// </summary>
    /// <param name="roles">The required roles.</param>
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
