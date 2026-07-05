using MediatR;
using System;

namespace Boksi.Application.Employees.Commands
{
    public class DeactivateEmployeeCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
    }
}
