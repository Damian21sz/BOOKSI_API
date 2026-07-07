using MediatR;
using System;

namespace Boksi.Application.Employees.Commands
{
    public class UpdateEmployeeProfileCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
    }
}
