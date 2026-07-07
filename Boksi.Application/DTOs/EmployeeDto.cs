using System;

namespace Boksi.Application.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // "Active" or "Inactive"
        public int VacationDaysLimit { get; set; }
        public int TargetMonthlyHours { get; set; }
        public System.Collections.Generic.List<Guid> Services { get; set; } = new System.Collections.Generic.List<Guid>();
        
        // Basic representation of schedule and timeoffs for frontend initial load
        public System.Collections.Generic.Dictionary<string, object> Schedule { get; set; } = new System.Collections.Generic.Dictionary<string, object>();
    }
}
