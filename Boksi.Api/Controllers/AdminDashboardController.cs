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
                    s.BaseSubscriptionPrice
                })
                .ToListAsync();

            return Ok(salons);
        }

        [HttpPost("discounts")]
        public async Task<IActionResult> CreateDiscountCode([FromBody] DiscountCode code)
        {
            _dbContext.DiscountCodes.Add(code);
            await _dbContext.SaveChangesAsync(default);
            return Ok(code);
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
    }
}
