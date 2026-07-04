using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public LoyaltyController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _dbContext.LoyaltyProgramSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new LoyaltyProgramSettings(); // Default
            }
            return Ok(settings);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] LoyaltyProgramSettings newSettings)
        {
            var settings = await _dbContext.LoyaltyProgramSettings.FirstOrDefaultAsync();
            
            if (settings == null)
            {
                _dbContext.LoyaltyProgramSettings.Add(newSettings);
            }
            else
            {
                settings.IsActive = newSettings.IsActive;
                settings.RequiredVisitsForReward = newSettings.RequiredVisitsForReward;
                settings.RewardDescription = newSettings.RewardDescription;
                settings.UpdatedAt = DateTime.UtcNow;
            }
            
            await _dbContext.SaveChangesAsync(default);
            return Ok(newSettings);
        }

        [HttpGet("cards/{clientId}")]
        public async Task<IActionResult> GetCard(Guid clientId)
        {
            var card = await _dbContext.ClientLoyaltyCards.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (card == null)
            {
                card = new ClientLoyaltyCard { ClientId = clientId };
            }
            return Ok(card);
        }

        [HttpPost("cards/{clientId}/add-visit")]
        public async Task<IActionResult> AddVisitPoint(Guid clientId)
        {
            var settings = await _dbContext.LoyaltyProgramSettings.FirstOrDefaultAsync();
            if (settings == null || !settings.IsActive)
            {
                return BadRequest("Program lojalnościowy nie jest aktywny w tym salonie.");
            }

            var card = await _dbContext.ClientLoyaltyCards.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (card == null)
            {
                card = new ClientLoyaltyCard { ClientId = clientId };
                _dbContext.ClientLoyaltyCards.Add(card);
            }

            card.CurrentPoints++;
            card.LastVisitAt = DateTime.UtcNow;

            string message = "Dodano wizytę.";

            if (card.CurrentPoints >= settings.RequiredVisitsForReward)
            {
                card.TotalRewardsEarned++;
                card.CurrentPoints = 0; // Reset after earning reward
                message = $"Gratulacje! Zdobycz nagrodę: {settings.RewardDescription}";
            }

            await _dbContext.SaveChangesAsync(default);
            return Ok(new { Card = card, Message = message });
        }
    }
}
