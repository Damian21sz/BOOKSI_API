using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyDashboardController : ControllerBase
    {
        private readonly Boksi.Application.Interfaces.IApplicationDbContext _dbContext;

        public CompanyDashboardController(Boksi.Application.Interfaces.IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("stats")]
        public async System.Threading.Tasks.Task<IActionResult> GetStats()
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
        public async System.Threading.Tasks.Task<IActionResult> GetAppointments()
        {
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest("Missing X-Tenant-Id header.");

            var today = System.DateTime.UtcNow.Date;

            // Fetch active employees
            var employees = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                System.Linq.Queryable.Where(_dbContext.Employees, e => e.IsActive && !e.IsDeleted));

            var result = new List<object>();

            foreach (var emp in employees)
            {
                var tasks = new List<object>();

                // Check for approved timeoff today
                var timeOff = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                    System.Linq.Queryable.Where(_dbContext.TimeOffs, 
                        t => t.EmployeeId == emp.Id && t.Status == Boksi.Domain.Entities.TimeOffStatus.Approved && !t.IsDeleted && t.StartDate <= today && t.EndDate >= today)
                );

                if (timeOff != null)
                {
                    tasks.Add(new {
                        Id = timeOff.Id.ToString(),
                        Time = "00:00",
                        Duration = 1440, // Whole day
                        Title = timeOff.Reason ?? "Urlop / Nieobecność",
                        Client = (string)null,
                        IsCustom = true
                    });
                }
                else
                {
                    // Fetch actual appointments for this employee today
                    var appointments = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                        System.Linq.Queryable.OrderBy(
                            System.Linq.Queryable.Where(_dbContext.Appointments, 
                                a => a.EmployeeId == emp.Id && a.StartTime.Date == today && !a.IsDeleted),
                            a => a.StartTime)
                    );

                    foreach(var app in appointments)
                    {
                        tasks.Add(new {
                            Id = app.Id.ToString(),
                            Time = app.StartTime.ToString("HH:mm"),
                            Duration = (int)(app.EndTime - app.StartTime).TotalMinutes,
                            Title = "Wizyta",
                            Client = "Klient Zarejestrowany", // We would include Client name
                            IsCustom = false
                        });
                    }
                }

                result.Add(new {
                    EmployeeId = emp.Id.ToString(),
                    EmployeeName = emp.FirstName + " " + emp.LastName,
                    Tasks = tasks
                });
            }

            return Ok(result);
        }
    }
}
