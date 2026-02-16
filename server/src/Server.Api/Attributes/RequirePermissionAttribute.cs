using Microsoft.AspNetCore.Authorization;

namespace Server.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string resource, string action)
    {
        Resource = resource;
        Action = action;
        Policy = $"Permission.{resource}.{action}";
    }

    public string Resource { get; }
    public string Action { get; }
}
