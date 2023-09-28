using Microsoft.AspNetCore.Authorization;

namespace PermissionBasedAuth.Authorization;

public class PermissionRequirement: IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
