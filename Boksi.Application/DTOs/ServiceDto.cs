using System;

namespace Boksi.Application.DTOs
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
