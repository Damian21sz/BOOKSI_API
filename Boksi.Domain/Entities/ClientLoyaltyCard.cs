using System;

namespace Boksi.Domain.Entities
{
    public class ClientLoyaltyCard : TenantEntity
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int CurrentPoints { get; set; } = 0;
        
        public int TotalRewardsEarned { get; set; } = 0;

        public DateTime LastVisitAt { get; set; } = DateTime.UtcNow;
    }
}
