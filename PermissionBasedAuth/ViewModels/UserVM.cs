using System.ComponentModel.DataAnnotations;

namespace PermissionBasedAuth.ViewModels;

public class UserVM
{
    public string Id { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
}
