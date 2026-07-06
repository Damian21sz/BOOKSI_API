using System;

namespace Boksi.Domain.Entities
{
    public class Changelog : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "news"; // e.g. news, changelog, announcement
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
