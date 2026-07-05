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
        public string Status { get; set; } // "Active" or "Inactive"
    }
}
