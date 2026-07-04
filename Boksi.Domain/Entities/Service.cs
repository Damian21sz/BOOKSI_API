using System;

namespace Boksi.Domain.Entities
{
    public class Service : TenantEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } = 30; // Domyślnie 30 minut

        public Guid CategoryId { get; set; }
        public ServiceCategory Category { get; set; } = null!;
    }
}
