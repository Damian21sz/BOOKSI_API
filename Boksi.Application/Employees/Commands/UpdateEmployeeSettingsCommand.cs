using MediatR;
using System;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeSettingsCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
        public int VacationDaysLimit { get; set; }
        public int TargetMonthlyHours { get; set; }
    }
}
