using System.Collections.Generic;

namespace Boksi.Domain.Entities
{
    public class BusinessCategory : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Salon> Salons { get; set; } = new List<Salon>();
    }
}
