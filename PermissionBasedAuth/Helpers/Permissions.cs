using PermissionBasedAuth.Models.Enums;

namespace PermissionBasedAuth.Helpers;

public class Permissions
{
    public static List<string> GenerateAllPermissions()
    {
        var permissions = new List<string>();
        var modules = Enum.GetValues(typeof(AppModules));

        foreach (var module in modules)
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

    public static class Users
    {
        public const string View = "Permission.Users.View";
        public const string Create = "Permission.Users.Create";
        public const string Update = "Permission.Users.Update";
        public const string Delete = "Permission.Users.Delete";
    }

    public static class Roles
    {
        public const string View = "Permission.Roles.View";
        public const string Create = "Permission.Roles.Create";
        public const string Update = "Permission.Roles.Update";
        public const string Delete = "Permission.Roles.Delete";
    }
}
