using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyDashboardController : ControllerBase
    {
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            // The X-Tenant-Id header determines which salon data we return.
            // For now, we mock the data.
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].ToString();

            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Missing X-Tenant-Id header.");
            }

            return Ok(new
            {
                RevenueToday = 1250,
                NewClients = 3,
                AppointmentsToday = 8
            });
        }

        [HttpGet("appointments")]
        public IActionResult GetAppointments()
        {
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].ToString();

            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Missing X-Tenant-Id header.");
            }

            var appointments = new List<object>
            {
                new { Time = "12:00", Duration = "60 min", Service = "Strzyżenie Męskie", Client = "Jan Kowalski", Employee = "Anna", Status = "Potwierdzona" },
                new { Time = "13:00", Duration = "30 min", Service = "Modelowanie", Client = "Katarzyna Nowak", Employee = "Piotr", Status = "Oczekująca" },
                new { Time = "14:30", Duration = "45 min", Service = "Farbowanie", Client = "Anna Maria", Employee = "Anna", Status = "Potwierdzona" }
            };

            return Ok(appointments);
        }
    }
}
