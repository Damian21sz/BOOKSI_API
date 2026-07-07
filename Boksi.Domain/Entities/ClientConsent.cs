using System;

namespace Boksi.Domain.Entities
{
    public class ClientConsent : SalonEntity
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public string ConsentType { get; set; } = null!; // np. RODO, Marketing_SMS, Marketing_Email
        public bool IsGranted { get; set; }
        public DateTime GrantedAt { get; set; }
    }
}
