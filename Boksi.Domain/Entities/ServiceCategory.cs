using System.Collections.Generic;

namespace Boksi.Domain.Entities
{
    public class ServiceCategory : TenantEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
