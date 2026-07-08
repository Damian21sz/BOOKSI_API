using System;

namespace Boksi.Application.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public string? ClientPhone { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public Guid? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
    }
}
