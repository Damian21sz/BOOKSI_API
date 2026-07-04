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
    public class WaitlistController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public WaitlistController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> JoinWaitlist([FromBody] WaitlistEntry entry)
        {
            // For now, simple mock assignment of ClientId if not provided
            if (entry.ClientId == Guid.Empty)
            {
                // In real app, we'd get this from User Claims
                entry.ClientId = Guid.NewGuid(); // Mock
            }

            _dbContext.WaitlistEntries.Add(entry);
            await _dbContext.SaveChangesAsync(default);
            return Ok(entry);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitlist()
        {
            var entries = await _dbContext.WaitlistEntries
                .Include(w => w.Client)
                .Include(w => w.Service)
                .OrderBy(w => w.Date)
                .ToListAsync();

            return Ok(entries);
        }
    }
}
