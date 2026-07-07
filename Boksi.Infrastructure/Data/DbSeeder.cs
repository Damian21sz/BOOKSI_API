using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Boksi.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "User", "SalonOwner", "SalonEmployee", "Sales", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await EnsureUserExists(userManager, "client@rivie.com", "User", "Jan", "Kowalski");
            await EnsureUserExists(userManager, "owner@rivie.com", "SalonOwner", "Anna", "Nowak");
            await EnsureUserExists(userManager, "employee@rivie.com", "SalonEmployee", "Piotr", "Wiśniewski");
            await EnsureUserExists(userManager, "sales@rivie.com", "Sales", "Kasia", "Wójcik");
            await EnsureUserExists(userManager, "admin@rivie.com", "Admin", "Super", "Admin");
        }

        private static async Task EnsureUserExists(UserManager<ApplicationUser> userManager, string email, string role, string firstName, string lastName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
