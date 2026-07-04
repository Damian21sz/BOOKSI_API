using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketingController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<MarketingController> _logger;

        public MarketingController(IApplicationDbContext dbContext, ILogger<MarketingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("campaigns")]
        public async Task<IActionResult> GetCampaigns()
        {
            var campaigns = await _dbContext.MarketingCampaigns
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(campaigns);
        }

        [HttpPost("campaigns")]
        public async Task<IActionResult> CreateCampaign([FromBody] MarketingCampaign campaign)
        {
            campaign.Status = MarketingCampaignStatus.Draft;
            _dbContext.MarketingCampaigns.Add(campaign);
            await _dbContext.SaveChangesAsync(default);

            return Ok(campaign);
        }

        [HttpPost("campaigns/{id}/send")]
        public async Task<IActionResult> SendCampaign(Guid id)
        {
            var campaign = await _dbContext.MarketingCampaigns.FirstOrDefaultAsync(c => c.Id == id);
            if (campaign == null) return NotFound();

            if (campaign.Status == MarketingCampaignStatus.Sent)
            {
                return BadRequest("Ta kampania została już wysłana.");
            }

            campaign.Status = MarketingCampaignStatus.Sent;
            campaign.SentAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(default);

            // Symulacja wysyłki
            _logger.LogInformation($"[MOCK MARKETING] Rozpoczęto wysyłkę kampanii '{campaign.Name}' (Typ: {campaign.Type}, Cel: {campaign.TargetCondition}).");
            _logger.LogInformation($"[MOCK MARKETING] Treść: {campaign.MessageTemplate}");
            _logger.LogInformation($"[MOCK MARKETING] Kampania zakończona pomyślnie.");

            return Ok(new { Message = "Kampania została wysłana." });
        }
    }
}
