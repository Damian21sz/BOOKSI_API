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

            var employeesWithTasks = new List<object>
            {
                new 
                { 
                    EmployeeId = "emp1", 
                    EmployeeName = "Anna", 
                    Tasks = new List<object>
                    {
                        new { Id = "t1", Time = "08:00", Duration = 60, Title = "Porządki", Client = (string)null, IsCustom = true },
                        new { Id = "t2", Time = "12:00", Duration = 60, Title = "Strzyżenie Męskie", Client = "Jan Kowalski", IsCustom = false }
                    }
                },
                new 
                { 
                    EmployeeId = "emp2", 
                    EmployeeName = "Piotr", 
                    Tasks = new List<object>
                    {
                        new { Id = "t3", Time = "13:00", Duration = 30, Title = "Modelowanie", Client = "Katarzyna Nowak", IsCustom = false },
                        new { Id = "t4", Time = "15:00", Duration = 120, Title = "Szkolenie BHP", Client = (string)null, IsCustom = true }
                    }
                }
            };

            return Ok(employeesWithTasks);
        }
    }
}
