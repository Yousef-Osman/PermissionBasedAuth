using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PermissionBasedAuth.Models.Enums;

namespace PermissionBasedAuth.Controllers;

[Authorize(Roles = nameof(UserRoles.SuperAdmin) + "," + nameof(UserRoles.Admin))]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
