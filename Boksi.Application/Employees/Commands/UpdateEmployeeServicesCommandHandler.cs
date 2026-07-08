using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeServicesCommandHandler : IRequestHandler<UpdateEmployeeServicesCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateEmployeeServicesCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateEmployeeServicesCommand request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.Services)
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null) return false;

            var newServices = await _dbContext.Services
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            employee.Services.Clear();
            foreach (var service in newServices)
            {
                employee.Services.Add(service);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
