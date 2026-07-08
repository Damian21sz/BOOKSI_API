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
        private readonly ICurrentUserService _currentUserService;

        public SetEmployeeScheduleCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(SetEmployeeScheduleCommand request, CancellationToken cancellationToken)
        {
            var startDate = new System.DateTime(request.Year, request.Month, 1, 0, 0, 0, System.DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            var existingSchedules = await _dbContext.EmployeeSchedules
                .Where(s => s.EmployeeId == request.EmployeeId && s.SpecificDate >= startDate && s.SpecificDate <= endDate)
                .ToListAsync(cancellationToken);

            _dbContext.EmployeeSchedules.RemoveRange(existingSchedules);

            foreach (var entry in request.Entries)
            {
                var schedule = new EmployeeSchedule
                {
                    Id = System.Guid.NewGuid(),
                    EmployeeId = request.EmployeeId,
                    SalonId = _currentUserService.SalonId ?? "default-salon",
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
