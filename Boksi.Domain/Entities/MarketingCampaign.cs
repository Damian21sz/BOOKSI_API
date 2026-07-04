using System;

namespace Boksi.Domain.Entities
{
    public enum MarketingCampaignType
    {
        SMS,
        Email
    }

    public enum MarketingCampaignTarget
    {
        AllClients,
        InactiveClients,
        LoyalClients
    }

    public enum MarketingCampaignStatus
    {
        Draft,
        Scheduled,
        Sent
    }

    public class MarketingCampaign : TenantEntity
    {
        public string Name { get; set; } = null!;
        
        public MarketingCampaignType Type { get; set; }
        
        public MarketingCampaignTarget TargetCondition { get; set; }
        
        public string MessageTemplate { get; set; } = null!;
        
        public MarketingCampaignStatus Status { get; set; } = MarketingCampaignStatus.Draft;
        
        public DateTime? SentAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
