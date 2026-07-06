using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangelogsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public ChangelogsController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "GlobalAdmin")]
        public async Task<IActionResult> GetAdminChangelogs()
        {
            var changelogs = await _dbContext.Changelogs
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(changelogs);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "GlobalAdmin")]
        public async Task<IActionResult> CreateChangelog([FromBody] Changelog changelog)
        {
            changelog.CreatedAt = DateTime.UtcNow;
            _dbContext.Changelogs.Add(changelog);
            await _dbContext.SaveChangesAsync(default);

            return Ok(changelog);
        }

        [HttpPut("admin/{id}")]
        [Authorize(Roles = "GlobalAdmin")]
        public async Task<IActionResult> UpdateChangelog(Guid id, [FromBody] Changelog updatedChangelog)
        {
            var changelog = await _dbContext.Changelogs.FindAsync(id);
            if (changelog == null) return NotFound();

            changelog.Title = updatedChangelog.Title;
            changelog.Content = updatedChangelog.Content;
            changelog.Type = updatedChangelog.Type;
            changelog.ImageUrl = updatedChangelog.ImageUrl;
            changelog.IsActive = updatedChangelog.IsActive;
            changelog.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(default);
            return Ok(changelog);
        }

        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "GlobalAdmin")]
        public async Task<IActionResult> DeleteChangelog(Guid id)
        {
            var changelog = await _dbContext.Changelogs.FindAsync(id);
            if (changelog == null) return NotFound();

            var receipts = await _dbContext.ChangelogReadReceipts.Where(r => r.ChangelogId == id).ToListAsync();
            _dbContext.ChangelogReadReceipts.RemoveRange(receipts);
            
            _dbContext.Changelogs.Remove(changelog);
            await _dbContext.SaveChangesAsync(default);
            
            return NoContent();
        }

        [HttpGet("latest")]
        [Authorize]
        public async Task<IActionResult> GetLatestChangelogs([FromQuery] int count = 3)
        {
            var userId = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty;
            
            var changelogs = await _dbContext.Changelogs
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();

            var changelogIds = changelogs.Select(c => c.Id).ToList();
            
            var readReceipts = await _dbContext.ChangelogReadReceipts
                .Where(r => r.UserId == userId && changelogIds.Contains(r.ChangelogId))
                .Select(r => r.ChangelogId)
                .ToListAsync();

            var result = changelogs.Select(c => new
            {
                c.Id,
                c.Title,
                c.Content,
                c.Type,
                c.ImageUrl,
                c.CreatedAt,
                IsRead = readReceipts.Contains(c.Id)
            });

            return Ok(result);
        }

        [HttpPost("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var existing = await _dbContext.ChangelogReadReceipts
                .FirstOrDefaultAsync(r => r.ChangelogId == id && r.UserId == userId);

            if (existing == null)
            {
                var receipt = new ChangelogReadReceipt
                {
                    ChangelogId = id,
                    UserId = userId,
                    ReadAtUtc = DateTime.UtcNow
                };
                _dbContext.ChangelogReadReceipts.Add(receipt);
                await _dbContext.SaveChangesAsync(default);
            }

            return Ok();
        }
    }
}
