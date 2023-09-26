using PermissionBasedAuth.Models.Enums;

namespace PermissionBasedAuth.Helpers;

public class PermissionManager
{
    public static List<string> GenerateAllPermissions()
    {
        var permissions = new List<string>();
        var modules = Enum.GetValues(typeof(AppModules));

        foreach ( var module in modules)
        {
            permissions.AddRange(new List<string>()
            {
                $"Permission.{module}.View",
                $"Permission.{module}.Create",
                $"Permission.{module}.Update",
                $"Permission.{module}.Delete",
            }); 
        }

        return permissions;
    }
}
