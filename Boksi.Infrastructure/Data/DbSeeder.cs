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
                    EmailConfirmed = true,
                    SalonId = (role == "SalonOwner" || role == "SalonEmployee") ? "11111111-1111-1111-1111-111111111111" : null
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public static async Task SeedSalonDataAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            string testSalonId = "11111111-1111-1111-1111-111111111111";

            Salon salon = dbContext.Salons.FirstOrDefault(s => s.Id == Guid.Parse(testSalonId));
            if (salon == null)
            {
                salon = new Salon { Id = Guid.Parse(testSalonId), Name = "Testowy Salon RIVIE" };
                dbContext.Salons.Add(salon);
                await dbContext.SaveChangesAsync(default);
            }

            if (!System.Linq.Queryable.Any(dbContext.Employees))
            {
                var catHair = new ServiceCategory { Id = Guid.NewGuid(), SalonId = testSalonId, Name = "Włosy - Koloryzacja" };
                var catNails = new ServiceCategory { Id = Guid.NewGuid(), SalonId = testSalonId, Name = "Paznokcie" };
                dbContext.ServiceCategories.AddRange(catHair, catNails);

                var s1 = new Service { Id = Guid.NewGuid(), SalonId = testSalonId, CategoryId = catHair.Id, Name = "Farbowanie odrostów", Price = 150, DurationMinutes = 60 };
                var s2 = new Service { Id = Guid.NewGuid(), SalonId = testSalonId, CategoryId = catNails.Id, Name = "Manicure Hybrydowy", Price = 120, DurationMinutes = 45 };
                dbContext.Services.AddRange(s1, s2);

                var e1 = new Employee { Id = Guid.NewGuid(), SalonId = testSalonId, FirstName = "Anna", LastName = "Nowak", JobTitle = "Senior Stylist", Email = "anna@rivie.com", IsActive = true, Services = new System.Collections.Generic.List<Service> { s1 } };
                var e2 = new Employee { Id = Guid.NewGuid(), SalonId = testSalonId, FirstName = "Piotr", LastName = "Wiśniewski", JobTitle = "Barber", Email = "piotr@rivie.com", IsActive = true, Services = new System.Collections.Generic.List<Service> { s1 } };
                var e3 = new Employee { Id = Guid.NewGuid(), SalonId = testSalonId, FirstName = "Kasia", LastName = "Kowalska", JobTitle = "Nail Artist", Email = "kasia@rivie.com", IsActive = true, Services = new System.Collections.Generic.List<Service> { s2 } };
                var e4 = new Employee { Id = Guid.NewGuid(), SalonId = testSalonId, FirstName = "Michał", LastName = "Wójcik", JobTitle = "Stylist", Email = "michal@rivie.com", IsActive = true, Services = new System.Collections.Generic.List<Service> { s1 } };
                var e5 = new Employee { Id = Guid.NewGuid(), SalonId = testSalonId, FirstName = "Zofia", LastName = "Krawczyk", JobTitle = "Junior Nail Artist", Email = "zofia@rivie.com", IsActive = true, Services = new System.Collections.Generic.List<Service> { s2 } };
                
                dbContext.Employees.AddRange(e1, e2, e3, e4, e5);
                await dbContext.SaveChangesAsync(default);

                // Add sample schedules and timeoffs
                var today = DateTime.UtcNow.Date;
                dbContext.EmployeeSchedules.Add(new EmployeeSchedule { Id = Guid.NewGuid(), SalonId = testSalonId, EmployeeId = e1.Id, SpecificDate = today, StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) });
                dbContext.EmployeeSchedules.Add(new EmployeeSchedule { Id = Guid.NewGuid(), SalonId = testSalonId, EmployeeId = e2.Id, SpecificDate = today, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(18) });

                dbContext.TimeOffs.Add(new TimeOff { Id = Guid.NewGuid(), SalonId = testSalonId, EmployeeId = e3.Id, StartDate = today.AddDays(1), EndDate = today.AddDays(5), Reason = "Urlop", Status = TimeOffStatus.Approved });

                dbContext.GalleryImages.Add(new GalleryImage { Id = Guid.NewGuid(), SalonId = testSalonId, EmployeeId = e1.Id, BeforeImageUrl = "https://images.unsplash.com/photo-1522337660859-02fbefca4702?w=500&auto=format&fit=crop", AfterImageUrl = "https://images.unsplash.com/photo-1595152772835-219674b2a8a6?w=500&auto=format&fit=crop" });

                await dbContext.SaveChangesAsync(default);
            }
        }
    }
}
