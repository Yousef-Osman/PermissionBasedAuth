namespace PermissionBasedAuth.ViewModels;

public class RolePermissionsVM
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public List<PermissionVM> Permissions { get; set; }
}
