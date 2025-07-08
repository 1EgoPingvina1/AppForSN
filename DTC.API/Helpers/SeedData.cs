using DTC.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DTC.API.Helpers
{
    public static class SeedData
    {
        private static readonly string[] Roles = new[] { "Admin", "User", "Reviewer", "Manager", "Author" };

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            foreach (var roleName in Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    });
                }
            }

            var adminEmail = "admin@admin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new User
                {
                    FirstName = "admin",
                    SecondName = "admin",
                    LastName = "admin",
                    Birthday = DateTime.UtcNow,
                    Gender = "male",
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    throw new Exception("Ошибка при создании администратора: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
