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
    public class ReviewsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IApplicationDbContext dbContext, ILogger<ReviewsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AppointmentReview review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Ocena musi być w przedziale 1-5.");
            }

            _dbContext.AppointmentReviews.Add(review);
            await _dbContext.SaveChangesAsync(default);

            return Ok(review);
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _dbContext.AppointmentReviews
                .Include(r => r.Appointment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpPost("{appointmentId}/send-request")]
        public IActionResult SendSurveyRequest(Guid appointmentId)
        {
            // Opcjonalnie: pobranie wizyty i telefonu klienta z bazy.
            // Z uwagi na wczesną fazę robimy tylko mock w logach.
            
            _logger.LogInformation($"[MOCK SMS/PUSH] Wysłano prośbę o ocenę wizyty dla AppointmentId: {appointmentId}. 'Dziękujemy za wizytę! Jak oceniasz usługę?'");

            return Ok(new { Message = "Powiadomienie z ankietą zostało wysłane pomyślnie." });
        }
    }
}
