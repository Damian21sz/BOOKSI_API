using System;

namespace Boksi.Domain.Entities
{
    public class WaitlistEntry : SalonEntity
    {
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }

        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        
        public bool IsNotified { get; set; } = false;
    }
}
