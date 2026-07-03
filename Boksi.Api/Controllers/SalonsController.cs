using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalonsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRecommendedSalons()
        {
            // Mock data for now, later will use MediatR to fetch from DB
            var salons = new List<object>
            {
                new { Id = "salon-1", Name = "Salon Piękności Venus", Address = "ul. Marszałkowska 12, Warszawa", Rating = 4.9, ReviewsCount = 120 },
                new { Id = "salon-2", Name = "Barber Shop Gentlemen", Address = "ul. Mokotowska 45, Warszawa", Rating = 4.8, ReviewsCount = 85 },
                new { Id = "salon-3", Name = "Studio Fryzur", Address = "ul. Nowy Świat 2, Warszawa", Rating = 4.7, ReviewsCount = 230 },
                new { Id = "salon-4", Name = "Spa & Wellness", Address = "ul. Złota 44, Warszawa", Rating = 5.0, ReviewsCount = 50 }
            };

            return Ok(salons);
        }
    }
}
