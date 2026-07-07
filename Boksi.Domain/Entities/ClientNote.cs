using System;

namespace Boksi.Domain.Entities
{
    public class ClientNote : SalonEntity
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public string Text { get; set; } = null!;
        public string? Author { get; set; }
    }
}
