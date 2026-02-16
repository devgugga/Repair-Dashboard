using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Server.Api.Authorization;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

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

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }
        }

        // Fall back to default policy provider
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
