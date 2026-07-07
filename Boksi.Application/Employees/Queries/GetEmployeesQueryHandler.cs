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
            var employeesEntities = await _dbContext.Employees
                .Include(e => e.Services)
                .ToListAsync(cancellationToken);

            var schedules = await _dbContext.EmployeeSchedules.ToListAsync(cancellationToken);

            var employees = employeesEntities.Select(e => {
                var empDto = new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    JobTitle = e.JobTitle,
                    PhotoUrl = e.PhotoUrl,
                    Description = e.Description,
                    VacationDaysLimit = e.VacationDaysLimit,
                    TargetMonthlyHours = e.TargetMonthlyHours,
                    Status = e.IsActive ? "Active" : "Inactive",
                    Services = e.Services.Select(s => s.Id).ToList(),
                    Schedule = new Dictionary<string, object>()
                };

                var empSchedules = schedules.Where(s => s.EmployeeId == e.Id && s.SpecificDate.HasValue).ToList();
                foreach (var sch in empSchedules)
                {
                    if (sch.StartTime.HasValue && sch.EndTime.HasValue)
                    {
                        var dateStr = sch.SpecificDate.Value.ToString("yyyy-MM-dd");
                        empDto.Schedule[dateStr] = new { start = sch.StartTime.Value.ToString(@"hh\:mm"), end = sch.EndTime.Value.ToString(@"hh\:mm") };
                    }
                }

                return empDto;
            }).ToList();

            return employees;
        }
    }
}
