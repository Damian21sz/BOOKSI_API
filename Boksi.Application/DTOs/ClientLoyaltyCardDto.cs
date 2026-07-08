using System;

namespace Boksi.Application.DTOs
{
    public class ClientLoyaltyCardDto
    {
        public Guid Id { get; set; }
        public string SalonId { get; set; } = null!;
        public string SalonName { get; set; } = null!;
        public int CurrentPoints { get; set; }
        public int TotalRewardsEarned { get; set; }
        public DateTime LastVisitAt { get; set; }
    }
}
