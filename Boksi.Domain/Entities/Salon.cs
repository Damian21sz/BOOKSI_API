using System.Collections.Generic;

namespace Boksi.Domain.Entities
{
    // Salon acts as our Tenant
    public class Salon : BaseEntity
    {
        public string Identifier { get; set; } = null!; // Used by Finbuckle as TenantId
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
