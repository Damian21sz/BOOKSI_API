using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "GlobalAdmin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public AdminDashboardController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalSalons = await _dbContext.Salons.CountAsync();
            var activeSalons = await _dbContext.Salons.CountAsync(s => s.SubscriptionStatus == SubscriptionStatus.Active);
            var trials = await _dbContext.Salons.CountAsync(s => s.SubscriptionStatus == SubscriptionStatus.Trial);
            var expired = await _dbContext.Salons.CountAsync(s => s.SubscriptionStatus == SubscriptionStatus.Expired);

            return Ok(new {
                TotalSalons = totalSalons,
                ActiveSubscriptions = activeSalons,
                InTrial = trials,
                Expired = expired
            });
        }

        [HttpGet("salons")]
        public async Task<IActionResult> GetSalons()
        {
            var salons = await _dbContext.Salons
                .Select(s => new {
                    s.Id,
                    s.Name,
                    s.Identifier,
                    s.SubscriptionStatus,
                    s.SubscriptionValidUntil,
                    s.BaseSubscriptionPrice,
                    PhoneNumber = s.PhoneNumber ?? "Brak",
                    Email = s.Identifier + "@example.com",
                    s.SalespersonId
                })
                .ToListAsync();

            return Ok(salons);
        }

        [HttpGet("discounts")]
        public async Task<IActionResult> GetDiscountCodes()
        {
            var codes = await _dbContext.DiscountCodes.ToListAsync();
            return Ok(codes);
        }

        [HttpPost("discounts")]
        public async Task<IActionResult> CreateDiscountCode([FromBody] DiscountCode code)
        {
            _dbContext.DiscountCodes.Add(code);
            await _dbContext.SaveChangesAsync(default);
            return Ok(code);
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            var plans = await _dbContext.SubscriptionPlans.ToListAsync();
            if (!plans.Any())
            {
                var defaultPlan = new SubscriptionPlan { Name = "Standard", Description = "Domyślny plan", PricePerMonth = 100.00m };
                _dbContext.SubscriptionPlans.Add(defaultPlan);
                await _dbContext.SaveChangesAsync(default);
                plans.Add(defaultPlan);
            }
            return Ok(plans);
        }

        [HttpPut("plans/{id}")]
        public async Task<IActionResult> UpdateSubscriptionPlan(Guid id, [FromBody] SubscriptionPlan planUpdate)
        {
            var plan = await _dbContext.SubscriptionPlans.FindAsync(id);
            if (plan == null) return NotFound();
            plan.PricePerMonth = planUpdate.PricePerMonth;
            plan.Name = planUpdate.Name;
            plan.Description = planUpdate.Description;
            await _dbContext.SaveChangesAsync(default);
            return Ok(plan);
        }

        [HttpPut("salons/{id}/price")]
        public async Task<IActionResult> UpdateBasePrice(Guid id, [FromBody] decimal newPrice)
        {
            var salon = await _dbContext.Salons.FindAsync(id);
            if (salon == null) return NotFound();

            salon.BaseSubscriptionPrice = newPrice;
            await _dbContext.SaveChangesAsync(default);
            return Ok();
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetBusinessCategories()
        {
            var categories = await _dbContext.BusinessCategories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateBusinessCategory([FromBody] BusinessCategory category)
        {
            _dbContext.BusinessCategories.Add(category);
            await _dbContext.SaveChangesAsync(default);
            return Ok(category);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateBusinessCategory(Guid id, [FromBody] BusinessCategory updatedCategory)
        {
            var category = await _dbContext.BusinessCategories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = updatedCategory.Name;
            category.Description = updatedCategory.Description;
            await _dbContext.SaveChangesAsync(default);
            return Ok(category);
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteBusinessCategory(Guid id)
        {
            var category = await _dbContext.BusinessCategories.FindAsync(id);
            if (category == null) return NotFound();

            _dbContext.BusinessCategories.Remove(category);
            await _dbContext.SaveChangesAsync(default);
            return NoContent();
        }

        [HttpGet("salespeople")]
        public async Task<IActionResult> GetSalespeople([FromServices] UserManager<ApplicationUser> userManager)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync("Salesperson");
            var salespeople = usersInRole.Select(u => new
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                CommissionPercentage = u.CommissionPercentage ?? 0m
            });
            return Ok(salespeople);
        }

        public class CreateSalespersonDto
        {
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public decimal CommissionPercentage { get; set; }
        }

        [HttpPost("salespeople")]
        public async Task<IActionResult> CreateSalesperson(
            [FromBody] CreateSalespersonDto request,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] RoleManager<IdentityRole> roleManager,
            [FromServices] IEmailService emailService)
        {
            // Ensure role exists
            if (!await roleManager.RoleExistsAsync("Salesperson"))
            {
                await roleManager.CreateAsync(new IdentityRole("Salesperson"));
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CommissionPercentage = request.CommissionPercentage,
                MustChangePassword = true
            };

            // Generate a random password
            var temporaryPassword = "P@ssw0rd!" + Guid.NewGuid().ToString().Substring(0, 4);

            var result = await userManager.CreateAsync(user, temporaryPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Salesperson");

            // Send email
            var emailBody = $"Zostałeś zarejestrowany jako Handlowiec.\nTwoje tymczasowe hasło to: {temporaryPassword}\nZmień hasło przy pierwszym logowaniu.";
            await emailService.SendEmailAsync(user.Email, "Rejestracja - RIVIE HQ", emailBody);

            return Ok(new { 
                Id = user.Id, 
                Name = $"{user.FirstName} {user.LastName}", 
                Email = user.Email, 
                CommissionPercentage = user.CommissionPercentage 
            });
        }

        public class UpdateCommissionDto { public decimal Percentage { get; set; } }

        [HttpPost("salespeople/{id}/commission")]
        public async Task<IActionResult> UpdateCommission(string id, [FromBody] UpdateCommissionDto request, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.CommissionPercentage = request.Percentage;
            await userManager.UpdateAsync(user);
            return Ok();
        }
    }
}
