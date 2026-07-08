using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Schedules.Queries
{
    public class GetAvailableTermsQuery : IRequest<List<AvailableTermDto>>
    {
        public Guid SalonId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime Date { get; set; }
    }

    public class GetAvailableTermsQueryHandler : IRequestHandler<GetAvailableTermsQuery, List<AvailableTermDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAvailableTermsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AvailableTermDto>> Handle(GetAvailableTermsQuery request, CancellationToken cancellationToken)
        {
            var targetDate = request.Date.Date;
            var dayOfWeek = targetDate.DayOfWeek;

            var salon = await _dbContext.Salons.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == request.SalonId, cancellationToken);
            if (salon == null) return new List<AvailableTermDto>();
            var tenantId = salon.Identifier;

            // Pobierz usługę, by sprawdzić jej czas trwania
            var service = await _dbContext.Services
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.SalonId == tenantId, cancellationToken);

            if (service == null) return new List<AvailableTermDto>();
            var duration = TimeSpan.FromMinutes(service.DurationMinutes);

            // Pobierz aktywnych pracowników
            var employees = await _dbContext.Employees
                .IgnoreQueryFilters()
                .Where(e => e.SalonId == tenantId && e.IsActive)
                .ToListAsync(cancellationToken);

            var availableTerms = new List<AvailableTermDto>();

            foreach (var employee in employees)
            {
                // Sprawdź urlop (TimeOff)
                var hasTimeOff = await _dbContext.TimeOffs
                    .IgnoreQueryFilters()
                    .AnyAsync(t => t.EmployeeId == employee.Id && t.Status == TimeOffStatus.Approved && t.StartDate.Date <= targetDate && t.EndDate.Date >= targetDate, cancellationToken);
                
                if (hasTimeOff) continue;

                // Sprawdź grafik
                var schedule = await _dbContext.EmployeeSchedules
                    .IgnoreQueryFilters()
                    .Where(s => s.EmployeeId == employee.Id && 
                                ((s.SpecificDate != null && s.SpecificDate.Value.Date == targetDate) || 
                                 (s.SpecificDate == null && s.DayOfWeek == dayOfWeek)))
                    .OrderByDescending(s => s.SpecificDate.HasValue) // SpecificDate ważniejsze niż cykliczne
                    .FirstOrDefaultAsync(cancellationToken);

                if (schedule == null || !schedule.IsWorkingDay || !schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
                {
                    continue; // Pracownik nie pracuje w ten dzień
                }

                // Pobierz istniejące wizyty
                var appointments = await _dbContext.Appointments
                    .IgnoreQueryFilters()
                    .Where(a => a.EmployeeId == employee.Id && a.StartTime.Date == targetDate && a.Status != AppointmentStatus.Cancelled)
                    .ToListAsync(cancellationToken);

                // Wygeneruj możliwe sloty, co np. 30 minut (dla uproszczenia), lub krok taki sam jak czas trwania usługi
                var step = TimeSpan.FromMinutes(30); 
                var currentTime = schedule.StartTime.Value;
                var endTime = schedule.EndTime.Value;

                while (currentTime.Add(duration) <= endTime)
                {
                    var slotStart = targetDate.Add(currentTime);
                    var slotEnd = slotStart.Add(duration);

                    // Sprawdź czy slot koliduje z jakąkolwiek wizytą
                    var hasConflict = appointments.Any(a => 
                        (slotStart >= a.StartTime && slotStart < a.EndTime) || // Slot zaczyna się w trakcie innej wizyty
                        (slotEnd > a.StartTime && slotEnd <= a.EndTime) ||     // Slot kończy się w trakcie innej wizyty
                        (slotStart <= a.StartTime && slotEnd >= a.EndTime)     // Slot zawiera inną wizytę
                    );

                    // Pomiń przeszłe terminy dla dnia dzisiejszego
                    if (!hasConflict && slotStart > DateTime.Now)
                    {
                        availableTerms.Add(new AvailableTermDto
                        {
                            EmployeeId = employee.Id,
                            EmployeeName = $"{employee.FirstName} {employee.LastName}",
                            StartTime = slotStart,
                            EndTime = slotEnd
                        });
                    }

                    currentTime = currentTime.Add(step);
                }
            }

            return availableTerms.OrderBy(t => t.StartTime).ToList();
        }
    }

    public class AvailableTermDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
