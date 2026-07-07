using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Schedules.Commands
{
    public class SetEmployeeScheduleCommandHandler : IRequestHandler<SetEmployeeScheduleCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public SetEmployeeScheduleCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(SetEmployeeScheduleCommand request, CancellationToken cancellationToken)
        {
            // Usunięcie starych wpisów dla pracownika ale tylko w danym miesiącu
            var startDate = new System.DateTime(request.Year, request.Month, 1, 0, 0, 0, System.DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            var existingSchedules = await _dbContext.EmployeeSchedules
                .Where(s => s.EmployeeId == request.EmployeeId && s.SpecificDate >= startDate && s.SpecificDate <= endDate)
                .ToListAsync(cancellationToken);

            _dbContext.EmployeeSchedules.RemoveRange(existingSchedules);

            // Dodanie nowych
            foreach (var entry in request.Entries)
            {
                var schedule = new EmployeeSchedule
                {
                    EmployeeId = request.EmployeeId,
                    DayOfWeek = entry.DayOfWeek,
                    SpecificDate = entry.SpecificDate,
                    IsWorkingDay = entry.IsWorkingDay,
                    StartTime = entry.StartTime,
                    EndTime = entry.EndTime
                };
                _dbContext.EmployeeSchedules.Add(schedule);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
