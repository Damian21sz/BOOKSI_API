using System;

namespace Boksi.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public abstract class SalonEntity : BaseEntity
    {
        public string SalonId { get; set; } = null!;
    }
}
