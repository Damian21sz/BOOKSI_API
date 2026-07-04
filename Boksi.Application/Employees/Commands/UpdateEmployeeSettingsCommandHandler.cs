using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeSettingsCommandHandler : IRequestHandler<UpdateEmployeeSettingsCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateEmployeeSettingsCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateEmployeeSettingsCommand request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null) return false;

            employee.VacationDaysLimit = request.VacationDaysLimit;
            employee.TargetMonthlyHours = request.TargetMonthlyHours;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
