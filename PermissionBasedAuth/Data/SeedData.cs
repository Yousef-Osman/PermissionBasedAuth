using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Models.Enums;

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

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var superAdmin = new ApplicationUser
        {
            FirstName = "Super",
            LastName = "Admin",
            UserName = "super_admin",
            Email = "superadmin@gmail.com",
            EmailConfirmed = true,
        };

        var user = await userManager.FindByEmailAsync(superAdmin.Email);

        if (user == null)
        {
            var result = await userManager.CreateAsync(superAdmin, "#Aaa123");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(superAdmin, nameof(UserRoles.SuperAdmin));
        }
    }
}
