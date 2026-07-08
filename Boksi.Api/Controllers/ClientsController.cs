using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public ClientsController(IApplicationDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        [HttpPost("me/favorites")]
        public async Task<IActionResult> AddFavoriteSalon([FromBody] Guid salonId)
        {
            // Mocking client ID for now
            var clientId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var existing = await _dbContext.FavoriteSalons
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.SalonId == salonId);

            if (existing != null)
            {
                return BadRequest("Salon is already in favorites.");
            }

            var favorite = new FavoriteSalon
            {
                ClientId = clientId,
                SalonId = salonId
            };

            _dbContext.FavoriteSalons.Add(favorite);
            await _dbContext.SaveChangesAsync(default);

            return Ok(favorite);
        }

        [HttpGet("me/favorites")]
        public async Task<IActionResult> GetFavoriteSalons()
        {
            // Mocking client ID for now
            var clientId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var favorites = await _dbContext.FavoriteSalons
                .Include(f => f.Salon)
                .Where(f => f.ClientId == clientId)
                .Select(f => f.Salon)
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpDelete("me/favorites/{salonId}")]
        public async Task<IActionResult> RemoveFavoriteSalon(Guid salonId)
        {
            var clientId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var favorite = await _dbContext.FavoriteSalons
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.SalonId == salonId);

            if (favorite == null) return NotFound();

            _dbContext.FavoriteSalons.Remove(favorite);
            await _dbContext.SaveChangesAsync(default);

            return NoContent();
        }

        [HttpGet("me/loyalty-cards")]
        public async Task<IActionResult> GetMyLoyaltyCards()
        {
            // Mocking client ID for now, just like in other methods
            var clientId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var query = new Boksi.Application.Clients.Queries.GetClientLoyaltyCardsQuery
            {
                ClientId = clientId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
