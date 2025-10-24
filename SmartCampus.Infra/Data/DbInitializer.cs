using Microsoft.AspNetCore.Identity;
using SmartCampus.Core.Entities;

public static class DbInitializer
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // Create roles if not exist
        var roles = new[] { "Admin", "Instructor", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }

        // Create admin if not exist
        if (await userManager.FindByEmailAsync("amal@gmail.com") == null)
        {
            var admin = new AppUser
            {
                UserName = "Amal",
                Email = "amal@gmail.com",
                FirstName = "Amal",
                LastName = "Ahmed",
                EmailConfirmed = true,
                Role = "Admin"
            };

            var result = await userManager.CreateAsync(admin, "Amal@2004");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
