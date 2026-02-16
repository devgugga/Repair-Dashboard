using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Server.Api.Authorization;

/// <summary>
///     Custom authorization policy provider that builds policies from permission names.
/// </summary>
public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    /// <summary>
    ///     Gets the default authorization policy.
    /// </summary>
    /// <returns>The default authorization policy task.</returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    /// <summary>
    ///     Gets the fallback authorization policy.
    /// </summary>
    /// <returns>The fallback authorization policy task.</returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    ///     Resolves a policy by name, including dynamic permission policies in the format:
    ///     Permission.{resource}.{action}.
    /// </summary>
    /// <param name="policyName">The policy name to resolve.</param>
    /// <returns>The resolved authorization policy task.</returns>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if it's a permission-based policy
        if (policyName.StartsWith("Permission.", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = policyName.Split('.');
            if (parts.Length == 3)
            {
                string resource = parts[1];
                string action = parts[2];

                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(resource, action))
                    .Build();

                // Intentionally synchronous: policy is built in-memory without async I/O.
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }
        }

        // Fall back to default policy provider
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
