using Microsoft.AspNetCore.Authorization;
using PermissionBasedAuth.Models.Enums;

namespace PermissionBasedAuth.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly string _issuer = "LOCAL AUTHORITY";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User != null)
        {
            var canAccess = context.User.Claims.Any(a => a.Type == ClaimType.Permission.ToString()
                            && a.Value == requirement.Permission && a.Issuer == _issuer);

            if (canAccess)
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
