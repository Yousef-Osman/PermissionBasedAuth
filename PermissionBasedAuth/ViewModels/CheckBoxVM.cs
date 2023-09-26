using System.ComponentModel.DataAnnotations;

namespace PermissionBasedAuth.ViewModels;

public class CheckBoxVM
{
    public string Id { get; set; }

    [Required]
    public virtual string Name { get; set; }

    public bool IsSelected { get; set; }
}

public class RoleVM: CheckBoxVM
{
    [Display(Name = "Role Name")]
    public override string Name { get; set; }
}

public class PermissionVM : CheckBoxVM
{
    [Display(Name = "Permission Name")]
    public override string Name { get; set; }
}