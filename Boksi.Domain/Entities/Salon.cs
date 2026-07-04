using System.Collections.Generic;

namespace Boksi.Domain.Entities
{
    public enum SubscriptionStatus
    {
        Trial,
        Active,
        Expired,
        Blocked
    }

    // Salon acts as our Tenant
    public class Salon : BaseEntity
    {
        public string Identifier { get; set; } = null!; // Used by Finbuckle as TenantId
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
        public System.DateTime SubscriptionValidUntil { get; set; } = System.DateTime.UtcNow.AddDays(14);
        public decimal BaseSubscriptionPrice { get; set; } = 100.00m; // Default price, could be overridden by plan/admin

        public System.Guid? BusinessCategoryId { get; set; }
        public BusinessCategory? Category { get; set; }
    }
}
