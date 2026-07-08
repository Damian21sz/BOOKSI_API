using MediatR;
using System;
using System.Collections.Generic;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeServicesCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
        public List<Guid> ServiceIds { get; set; } = new List<Guid>();
    }
}
