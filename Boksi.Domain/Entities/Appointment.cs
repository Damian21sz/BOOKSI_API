using System;

namespace Boksi.Domain.Entities
{
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    public class Appointment : TenantEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid? ServiceId { get; set; }
        public Service? Service { get; set; }

        public string? CustomTaskName { get; set; }
        public string? CustomTaskDescription { get; set; }

        public Guid? GroupId { get; set; } // Used for group bookings
    }
}
