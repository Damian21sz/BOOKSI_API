using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Employees.Commands
{
    public class DeactivateEmployeeCommandHandler : IRequestHandler<DeactivateEmployeeCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeactivateEmployeeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeactivateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);
            if (employee == null) return false;

            // Toggle logic: If active make inactive, if inactive make active.
            employee.IsActive = !employee.IsActive;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
