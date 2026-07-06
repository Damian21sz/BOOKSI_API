using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Salesperson")]
    public class SalesDashboardController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SalesDashboardController(IApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var commissionPercentage = user.CommissionPercentage ?? 0m;

            var salons = await _dbContext.Salons
                .Where(s => s.SalespersonId == userId)
                .ToListAsync();

            var activeCount = salons.Count(s => s.SubscriptionStatus == SubscriptionStatus.Active);
            var trialCount = salons.Count(s => s.SubscriptionStatus == SubscriptionStatus.Trial);
            
            // Estymowany przychód = suma (cena bazowa * procent prowizji) tylko dla aktywnych
            var estimatedRevenue = salons
                .Where(s => s.SubscriptionStatus == SubscriptionStatus.Active)
                .Sum(s => s.BaseSubscriptionPrice * (commissionPercentage / 100m));

            return Ok(new
            {
                EstimatedRevenue = estimatedRevenue,
                CommissionPercentage = commissionPercentage,
                DemoCompaniesCount = trialCount,
                ActiveCompaniesCount = activeCount,
                TotalCompanies = salons.Count
            });
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var commissionPercentage = user?.CommissionPercentage ?? 0m;

            var salons = await _dbContext.Salons
                .Where(s => s.SalespersonId == userId)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Identifier = s.Identifier,
                    Status = s.SubscriptionStatus.ToString(),
                    MonthlyPrice = s.BaseSubscriptionPrice,
                    Commission = s.SubscriptionStatus == SubscriptionStatus.Active 
                        ? s.BaseSubscriptionPrice * (commissionPercentage / 100m) 
                        : 0m,
                    PhoneNumber = s.PhoneNumber,
                    SubscriptionValidUntil = s.SubscriptionValidUntil
                })
                .ToListAsync();

            return Ok(salons);
        }
    }
}
