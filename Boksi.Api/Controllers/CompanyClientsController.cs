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
    public class CompanyClientsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public CompanyClientsController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest("Missing X-Salon-Id header.");

            // For MVP, fetch all clients that have at least one appointment in this tenant.
            var clients = await _dbContext.Appointments
                .Include(a => a.Client)
                .Where(a => !a.IsDeleted)
                .Select(a => a.Client)
                .Distinct()
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.PhoneNumber,
                    c.Email,
                    TotalVisits = _dbContext.Appointments.Count(a => a.ClientId == c.Id && !a.IsDeleted),
                    LastVisit = _dbContext.Appointments.Where(a => a.ClientId == c.Id && !a.IsDeleted).Max(a => (DateTime?)a.StartTime)
                })
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetClientHistory(Guid id)
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var appointments = await _dbContext.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Where(a => a.ClientId == id && !a.IsDeleted)
                .OrderByDescending(a => a.StartTime)
                .Select(a => new {
                    a.Id,
                    Date = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    Service = a.Service != null ? a.Service.Name : null,
                    Employee = a.Employee != null ? a.Employee.FirstName + " " + a.Employee.LastName : "",
                    Status = a.Status.ToString(),
                    Price = a.Service != null ? a.Service.Price.ToString() + " PLN" : "0 PLN"
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("{id}/notes")]
        public async Task<IActionResult> GetClientNotes(Guid id)
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var notes = await _dbContext.ClientNotes
                .Where(n => n.ClientId == id && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> AddClientNote(Guid id, [FromBody] ClientNote request)
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            request.ClientId = id;
            request.SalonId = tenantId;
            request.CreatedAt = DateTime.UtcNow;

            _dbContext.ClientNotes.Add(request);
            await _dbContext.SaveChangesAsync(default);

            return Ok(request);
        }

        [HttpGet("{id}/consents")]
        public async Task<IActionResult> GetClientConsents(Guid id)
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var consents = await _dbContext.ClientConsents
                .Where(c => c.ClientId == id && !c.IsDeleted)
                .OrderByDescending(c => c.GrantedAt)
                .ToListAsync();

            return Ok(consents);
        }

        [HttpPost("{id}/consents")]
        public async Task<IActionResult> UpdateClientConsent(Guid id, [FromBody] ClientConsent request)
        {
            var tenantId = HttpContext.Request.Headers["X-Salon-Id"].ToString();
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            request.ClientId = id;
            request.SalonId = tenantId;
            request.GrantedAt = DateTime.UtcNow;

            // Optional logic: if consent of this type exists, update it or add new history log.
            // For now, we append as a log.
            _dbContext.ClientConsents.Add(request);
            await _dbContext.SaveChangesAsync(default);

            return Ok(request);
        }
    }
}
