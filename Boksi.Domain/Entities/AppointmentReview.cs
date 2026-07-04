using System;
using System.ComponentModel.DataAnnotations;

namespace Boksi.Domain.Entities
{
    public class AppointmentReview : TenantEntity
    {
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
