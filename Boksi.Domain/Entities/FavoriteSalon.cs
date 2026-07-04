using System;

namespace Boksi.Domain.Entities
{
    public class FavoriteSalon : BaseEntity
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid SalonId { get; set; }
        public Salon Salon { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
