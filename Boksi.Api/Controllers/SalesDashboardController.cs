using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesDashboardController : ControllerBase
    {
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            // Zwracamy przykładowe statystyki dla Handlowca.
            // Docelowo: Pobieramy UserId z tokena, szukamy w bazie Salonów gdzie SalespersonId == UserId.
            return Ok(new
            {
                EstimatedRevenue = 450.00,
                CommissionPercentage = 20.0,
                DemoCompaniesCount = 3,
                ActiveCompaniesCount = 2
            });
        }

        [HttpGet("companies")]
        public IActionResult GetCompanies()
        {
            return Ok(new List<object>
            {
                new { Id = "c1", Name = "Salon Urody Bellissima", Status = "Active", MonthlyPrice = 100.00, Commission = 20.00 },
                new { Id = "c2", Name = "Barber Shop Classic", Status = "Active", MonthlyPrice = 150.00, Commission = 30.00 },
                new { Id = "c3", Name = "Studio Paznokci Glow", Status = "Trial", MonthlyPrice = 100.00, Commission = 0.00 },
                new { Id = "c4", Name = "Fryzjer Męski 'Ostre Cięcie'", Status = "Trial", MonthlyPrice = 100.00, Commission = 0.00 }
            });
        }
    }
}
