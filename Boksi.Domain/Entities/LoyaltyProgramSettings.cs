using System;

namespace Boksi.Domain.Entities
{
    public class LoyaltyProgramSettings : SalonEntity
    {
        public bool IsActive { get; set; } = false;
        
        public int RequiredVisitsForReward { get; set; } = 6;
        
        public string RewardDescription { get; set; } = "Darmowa usługa";
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
