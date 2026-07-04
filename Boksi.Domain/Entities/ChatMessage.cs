using System;

namespace Boksi.Domain.Entities
{
    public class ChatMessage : TenantEntity
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // The ID of the actual sender (Client or Employee)
        public Guid SenderId { get; set; }
        
        // "Client" or "Salon"
        public string SenderRole { get; set; } = null!;

        public string Content { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
