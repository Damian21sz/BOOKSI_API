using Boksi.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Boksi.Application.Employees.Queries
{
    public class GetEmployeesQuery : IRequest<List<EmployeeDto>>
    {
    }
}
