using MediatR;
using System;
using System.Collections.Generic;

namespace Boksi.Application.Schedules.Commands
{
    public class ScheduleEntryDto
    {
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? SpecificDate { get; set; }
        public bool IsWorkingDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class SetEmployeeScheduleCommand : IRequest<bool>
    {
        public List<Guid> EmployeeIds { get; set; } = new List<Guid>();
        public int Year { get; set; }
        public int Month { get; set; }
        public List<ScheduleEntryDto> Entries { get; set; } = new List<ScheduleEntryDto>();
    }
}
