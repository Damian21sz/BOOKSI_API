using System;

namespace Boksi.Domain.Entities
{
    public enum TimeOffStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class TimeOff : TenantEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string? Reason { get; set; }
        public TimeOffStatus Status { get; set; } = TimeOffStatus.Pending;
    }
}
