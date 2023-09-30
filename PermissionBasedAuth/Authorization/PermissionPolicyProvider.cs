using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using PermissionBasedAuth.Models.Enums;
using System.Net;

namespace PermissionBasedAuth.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public  DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        //a default authorization policy provider (constructed with options from the dependency injection container "othor policies")
        //is used if this custom provider isn't able to handle a given policy name.
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    //returns the policy used for [Authorize] attributes without a policy specified
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return FallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    //returns the policy used by the Authorization Middleware when no policy is specified
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return FallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        //ASP.NET Core only uses one instance of IAuthorizationPolicyProvider.
        //If a custom provider isn't able to provide authorization policies for all policy names,
        //it should defer to a backup provider (ex: FallbackPolicyProvider).
        if (policyName.StartsWith(ClaimType.Permission.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName));
            return Task.FromResult(policy.Build());
        }

        //returns an authorization policy for a given name (same as the method we are currently in).
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
