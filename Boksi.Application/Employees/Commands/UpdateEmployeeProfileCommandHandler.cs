using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeProfileCommandHandler : IRequestHandler<UpdateEmployeeProfileCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateEmployeeProfileCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateEmployeeProfileCommand request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null) return false;

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.JobTitle = request.JobTitle;
            employee.PhotoUrl = request.PhotoUrl;
            employee.Description = request.Description;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
