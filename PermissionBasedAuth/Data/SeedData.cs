using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Helpers;
using PermissionBasedAuth.Models.Enums;
using System.Security.Claims;

namespace PermissionBasedAuth.Data;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new IdentityRole(nameof(UserRoles.SuperAdmin)));
            await roleManager.CreateAsync(new IdentityRole(nameof(UserRoles.Admin)));
            await roleManager.CreateAsync(new IdentityRole(nameof(UserRoles.User)));
        }
    }

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var superAdminUser = new ApplicationUser
        {
            FirstName = "Super",
            LastName = "Admin",
            UserName = "super_admin",
            Email = "superadmin@gmail.com",
            EmailConfirmed = true,
        };

        var user = await userManager.FindByEmailAsync(superAdminUser.Email);

        if (user == null)
        {
            var result = await userManager.CreateAsync(superAdminUser, "#Aaa123");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(superAdminUser, nameof(UserRoles.SuperAdmin));
        }

        var superAdminRole = await roleManager.FindByNameAsync(UserRoles.SuperAdmin.ToString());
        await roleManager.AddAllPermissionsToRole(superAdminRole);
    }

    public static async Task AddAllPermissionsToRole(this RoleManager<IdentityRole> roleManager, IdentityRole role)
    {
        var currentPermissions = await roleManager.GetClaimsAsync(role);
        var allPermissions = PermissionManager.GenerateAllPermissions();

        foreach (var permission in allPermissions)
        {
            if (!currentPermissions.Any(a => a.Type == ClaimType.Permission.ToString() && a.Value == permission))
                await roleManager.AddClaimAsync(role, new Claim(ClaimType.Permission.ToString(), permission));
        }
    }
}
