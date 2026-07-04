using System;

namespace Boksi.Domain.Entities
{
    // Opcjonalnie rozszerzone plany w przyszłości, aktualnie jako jeden bazowy.
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PricePerMonth { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }

    public class DiscountCode : BaseEntity
    {
        public string Code { get; set; } = null!;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        
        // Null means global code for anyone
        public Guid? SpecificSalonId { get; set; }
        
        public int MaxUses { get; set; } = 1;
        public int CurrentUses { get; set; } = 0;
        
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
