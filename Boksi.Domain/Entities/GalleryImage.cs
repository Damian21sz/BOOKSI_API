using System;

namespace Boksi.Domain.Entities
{
    public class GalleryImage : SalonEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public string? BeforeImageUrl { get; set; }
        public string? AfterImageUrl { get; set; }
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
