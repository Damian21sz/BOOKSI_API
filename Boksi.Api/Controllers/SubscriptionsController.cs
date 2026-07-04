using Boksi.Application.Interfaces;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ITenantInfo _tenantInfo;

        public SubscriptionsController(IApplicationDbContext dbContext, ITenantInfo tenantInfo)
        {
            _dbContext = dbContext;
            _tenantInfo = tenantInfo;
        }

        [HttpPost("renew")]
        public async Task<IActionResult> RenewSubscription([FromBody] RenewSubscriptionRequest request)
        {
            if (_tenantInfo == null || string.IsNullOrEmpty(_tenantInfo.Identifier))
                return BadRequest("Brak kontekstu najemcy.");

            var salon = await _dbContext.Salons.FirstOrDefaultAsync(s => s.Identifier == _tenantInfo.Identifier);
            if (salon == null) return NotFound("Nie znaleziono salonu.");

            // Basic virtual renewal logic
            decimal finalPrice = salon.BaseSubscriptionPrice;

            if (!string.IsNullOrEmpty(request.DiscountCode))
            {
                var code = await _dbContext.DiscountCodes.FirstOrDefaultAsync(c => c.Code == request.DiscountCode && c.IsActive);
                if (code == null) return BadRequest("Nieprawidłowy kod rabatowy.");
                
                if (code.ExpiresAt.HasValue && code.ExpiresAt.Value < DateTime.UtcNow)
                    return BadRequest("Kod rabatowy wygasł.");

                if (code.CurrentUses >= code.MaxUses)
                    return BadRequest("Limit użyć kodu został wyczerpany.");

                if (code.SpecificSalonId.HasValue && code.SpecificSalonId.Value != salon.Id)
                    return BadRequest("Ten kod nie jest przypisany do Twojego salonu.");

                if (code.Type == Domain.Entities.DiscountType.Percentage)
                    finalPrice = finalPrice - (finalPrice * (code.Value / 100));
                else
                    finalPrice = finalPrice - code.Value;

                if (finalPrice < 0) finalPrice = 0;

                code.CurrentUses++;
            }

            // In real app, integrate Stripe here with finalPrice

            salon.SubscriptionValidUntil = DateTime.UtcNow.AddMonths(1);
            salon.SubscriptionStatus = Domain.Entities.SubscriptionStatus.Active;

            await _dbContext.SaveChangesAsync(default);

            return Ok(new { Message = "Subskrypcja została przedłużona.", NewExpiry = salon.SubscriptionValidUntil, Paid = finalPrice });
        }
    }

    public class RenewSubscriptionRequest
    {
        public string? DiscountCode { get; set; }
    }
}
