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
    public class TimeOffsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public TimeOffsController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var pending = await _dbContext.TimeOffs
                .Include(t => t.Employee)
                .Where(t => t.Status == TimeOffStatus.Pending && !t.IsDeleted)
                .Select(t => new {
                    t.Id,
                    t.EmployeeId,
                    EmployeeName = t.Employee.FirstName + " " + t.Employee.LastName,
                    t.StartDate,
                    t.EndDate,
                    t.Reason,
                    t.Status,
                    t.CreatedAt
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(pending);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(Guid employeeId)
        {
            var timeOffs = await _dbContext.TimeOffs
                .Where(t => t.EmployeeId == employeeId && !t.IsDeleted)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();

            return Ok(timeOffs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimeOff request)
        {
            if (request.StartDate >= request.EndDate)
                return BadRequest("Data zakończenia musi być późniejsza niż data rozpoczęcia.");

            request.Status = TimeOffStatus.Pending;
            _dbContext.TimeOffs.Add(request);
            await _dbContext.SaveChangesAsync(default);

            return Ok(request);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var timeOff = await _dbContext.TimeOffs.FirstOrDefaultAsync(t => t.Id == id);
            if (timeOff == null) return NotFound();

            timeOff.Status = TimeOffStatus.Approved;
            await _dbContext.SaveChangesAsync(default);
            return Ok();
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var timeOff = await _dbContext.TimeOffs.FirstOrDefaultAsync(t => t.Id == id);
            if (timeOff == null) return NotFound();

            timeOff.Status = TimeOffStatus.Rejected;
            await _dbContext.SaveChangesAsync(default);
            return Ok();
        }
    }
}
