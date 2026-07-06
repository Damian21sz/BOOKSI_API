using System;

namespace Boksi.Domain.Entities
{
    public class ChangelogReadReceipt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChangelogId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime ReadAtUtc { get; set; } = DateTime.UtcNow;

        // Navigation properties if needed
        public Changelog Changelog { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
