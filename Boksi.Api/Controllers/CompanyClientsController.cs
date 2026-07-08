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
        private readonly ICurrentUserService _currentUserService;

        public CompanyClientsController(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest("Missing Salon context.");

            // Fetch all clients that have at least one appointment in this tenant.
            var clients = await _dbContext.Appointments
                .Include(a => a.Client)
                .Where(a => a.SalonId == tenantId && !a.IsDeleted)
                .Select(a => a.Client)
                .Distinct()
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.PhoneNumber,
                    c.Email,
                    TotalVisits = _dbContext.Appointments.Count(a => a.ClientId == c.Id && a.SalonId == tenantId && !a.IsDeleted),
                    LastVisit = _dbContext.Appointments.Where(a => a.ClientId == c.Id && a.SalonId == tenantId && !a.IsDeleted).Max(a => (DateTime?)a.StartTime)
                })
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientDetails(Guid id)
        {
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest("Missing Salon context.");

            var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null) return NotFound();

            var totalVisits = await _dbContext.Appointments.CountAsync(a => a.ClientId == id && a.SalonId == tenantId && !a.IsDeleted);
            var lastVisit = await _dbContext.Appointments.Where(a => a.ClientId == id && a.SalonId == tenantId && !a.IsDeleted).MaxAsync(a => (DateTime?)a.StartTime);
            
            // Calculate total spent only from finished appointments or just sum of prices
            var totalSpent = await _dbContext.Appointments
                .Include(a => a.Service)
                .Where(a => a.ClientId == id && a.SalonId == tenantId && !a.IsDeleted && a.Status == AppointmentStatus.Completed && a.Service != null)
                .SumAsync(a => a.Service.Price);

            // Mock registered since as first visit date
            var firstVisit = await _dbContext.Appointments.Where(a => a.ClientId == id && a.SalonId == tenantId && !a.IsDeleted).MinAsync(a => (DateTime?)a.StartTime);

            return Ok(new
            {
                client.Id,
                client.FirstName,
                client.LastName,
                client.PhoneNumber,
                client.Email,
                TotalVisits = totalVisits,
                LastVisit = lastVisit,
                TotalSpent = $"{totalSpent} PLN",
                RegisteredSince = firstVisit?.ToString("yyyy-MM-dd") ?? "Brak"
            });
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetClientHistory(Guid id)
        {
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var appointments = await _dbContext.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Where(a => a.ClientId == id && a.SalonId == tenantId && !a.IsDeleted)
                .OrderByDescending(a => a.StartTime)
                .Select(a => new {
                    a.Id,
                    Date = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    Service = a.Service != null ? a.Service.Name : "Brak",
                    Employee = a.Employee != null ? a.Employee.FirstName + " " + a.Employee.LastName : "",
                    Status = a.Status == AppointmentStatus.Completed ? "Zakończona" : (a.Status == AppointmentStatus.Cancelled ? "Odwołana" : "Nadchodząca"),
                    Price = a.Service != null ? a.Service.Price.ToString() + " PLN" : "0 PLN"
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("{id}/notes")]
        public async Task<IActionResult> GetClientNotes(Guid id)
        {
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var notes = await _dbContext.ClientNotes
                .Where(n => n.ClientId == id && n.SalonId == tenantId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new {
                    n.Id,
                    Date = n.CreatedAt.ToString("yyyy-MM-dd"),
                    n.Text,
                    Author = n.Author ?? "System"
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> AddClientNote(Guid id, [FromBody] ClientNote request)
        {
            var tenantId = _currentUserService.SalonId;
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
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var consents = await _dbContext.ClientConsents
                .Where(c => c.ClientId == id && c.SalonId == tenantId && !c.IsDeleted)
                .OrderByDescending(c => c.GrantedAt)
                .Select(c => new {
                    c.Id,
                    type = c.ConsentType,
                    Date = c.GrantedAt.ToString("yyyy-MM-dd"),
                    granted = c.IsGranted
                })
                .ToListAsync();

            return Ok(consents);
        }

        public class ConsentDto
        {
            public string Type { get; set; }
            public bool Granted { get; set; }
        }

        [HttpPost("{id}/consents")]
        public async Task<IActionResult> UpdateClientConsent(Guid id, [FromBody] ConsentDto request)
        {
            var tenantId = _currentUserService.SalonId;
            if (string.IsNullOrEmpty(tenantId)) return BadRequest();

            var consent = new ClientConsent
            {
                ClientId = id,
                SalonId = tenantId,
                ConsentType = request.Type,
                IsGranted = request.Granted,
                GrantedAt = DateTime.UtcNow
            };

            _dbContext.ClientConsents.Add(consent);
            await _dbContext.SaveChangesAsync(default);

            return Ok(consent);
        }
    }
}
