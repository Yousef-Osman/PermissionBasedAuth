using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Data;
using PermissionBasedAuth.Helpers;
using PermissionBasedAuth.Models.Enums;
using PermissionBasedAuth.ViewModels;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace PermissionBasedAuth.Controllers;

[Authorize(Roles = nameof(UserRoles.SuperAdmin))]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(ApplicationDbContext context,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.Select(a => new UserVM
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            Email = a.Email,
            UserName = a.UserName
        }).ToListAsync();

        return View(users);
    }

    public async Task<IActionResult> EditUserRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return BadRequest();

        var roles = await _roleManager.Roles.ToListAsync();

        var viewModel = new UserRolesVM()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = roles.Select(role => new RoleVM
            {
                Id = role.Id,
                Name = role.Name,
                IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
            }).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserRole(UserRolesVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return BadRequest();

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in model.Roles)
        {
            if (userRoles.Contains(role.Name) && !role.IsSelected)
                await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (!userRoles.Contains(role.Name) && role.IsSelected)
                await _userManager.AddToRoleAsync(user, role.Name);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Roles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(RoleVM model)
    {
        if (!ModelState.IsValid)
            return Json(new JsonResponse { IsSuccess = false, Errors = GetErrorList(ModelState) });

        if (await _roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError("Name", "Role exists");
            return Json(new JsonResponse { IsSuccess = false, Errors = GetErrorList(ModelState) });
        }

        await _roleManager.CreateAsync(new IdentityRole(model.Name));

        return Ok(new JsonResponse { IsSuccess = true });
    }

    public async Task<IActionResult> EditRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return BadRequest();

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(IdentityRole model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = await _roleManager.FindByIdAsync(model.Id);

        if (role == null)
        {
            ModelState.AddModelError("Name", "no such Role exists");
            return View(model);
        }

        role.Name = model.Name;
        role.NormalizedName = model.Name.ToUpper();
        await _roleManager.UpdateAsync(role);

        return RedirectToAction(nameof(Roles));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole([FromBody] RoleVM model)
    {
        if (string.IsNullOrWhiteSpace(model?.Id))
            return Json(new JsonResponse { IsSuccess = false, ErrorMessage = "Invalid role" });

        var role = await _roleManager.FindByIdAsync(model.Id);
        if (role == null)
            return Json(new JsonResponse { IsSuccess = false, ErrorMessage = "Role doesn't exists." });

        var exists = _context.UserRoles.Any(a => a.RoleId == role.Id);
        if (exists)
            return Json(new JsonResponse { IsSuccess = false, ErrorMessage = "Can't delete role." });

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return Ok(new JsonResponse { IsSuccess = true });
        else
            return Json(new JsonResponse { IsSuccess = false, ErrorMessage = "Something went wrong." });
    }

    public async Task<IActionResult> EditRolePermissions(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return BadRequest();

        var currentPermissions = _roleManager.GetClaimsAsync(role).Result
           .Where(a => a.Type == ClaimType.Permission.ToString()).Select(a => a.Value).ToList();

        var allPermissions = PermissionManager.GenerateAllPermissions();

        var viewModel = new RolePermissionsVM()
        {
            RoleId = role.Id,
            RoleName = role.Name,
            Permissions = allPermissions.Select(permission => new PermissionVM
            {
                Name = permission,
                IsSelected = currentPermissions.Contains(permission)
            }).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRolePermissions(RolePermissionsVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = await _roleManager.FindByIdAsync(model.RoleId);

        if (role == null)
            return BadRequest();

        var currentRolePermissions = await _roleManager.GetClaimsAsync(role);
        var currentPermissions = currentRolePermissions.Where(a => a.Type == ClaimType.Permission.ToString())
            .Select(a => a.Value).ToList();

        foreach (var permission in model.Permissions)
        {
            if (currentPermissions.Contains(permission.Name) && !permission.IsSelected)
            {
                var claim = currentRolePermissions.FirstOrDefault(a => a.Value == permission.Name);
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            if (!currentPermissions.Contains(permission.Name) && permission.IsSelected)
                await _roleManager.AddClaimAsync(role, new Claim(ClaimType.Permission.ToString(), permission.Name));
        }

        return RedirectToAction(nameof(Roles));
    }

    private List<string> GetErrorList(ModelStateDictionary modelState)
    {
        return modelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
    }
}
