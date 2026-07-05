using Boksi.Application.DTOs;
using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Employees.Queries
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetEmployeesQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _dbContext.Employees
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    JobTitle = e.JobTitle,
                    Status = e.IsActive ? "Active" : "Inactive"
                })
                .ToListAsync(cancellationToken);

            return employees;
        }
    }
}
