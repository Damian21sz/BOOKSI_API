using System;

namespace Boksi.Domain.Entities
{
    public class EmployeeSchedule : TenantEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        // Jeśli ma wartość, to wpis jest cykliczny (np. w każdy poniedziałek).
        public DayOfWeek? DayOfWeek { get; set; }

        // Jeśli ma wartość, to wpis dotyczy tylko tego konkretnego dnia (np. 15 lipca). Nadrzędny wobec cyklicznych.
        public DateTime? SpecificDate { get; set; }

        // Czy pracownik pracuje w ten dzień. 
        // Jeśli false i mamy SpecificDate, oznacza to jednodniowe wolne (bez zużywania urlopu, po prostu brak grafiku).
        public bool IsWorkingDay { get; set; } = true;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
