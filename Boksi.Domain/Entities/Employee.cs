using System;
using System.Collections.Generic;

namespace Boksi.Domain.Entities
{
    public class Employee : SalonEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }

        public string? ApplicationUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // Ustawienia pracownika (limity)
        public int VacationDaysLimit { get; set; } = 26; // Domyślnie 26 dni urlopu
        public int TargetMonthlyHours { get; set; } = 160; // Domyślnie 160 godzin w miesiącu

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
