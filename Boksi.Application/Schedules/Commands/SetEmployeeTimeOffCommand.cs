using MediatR;
using System;
using Boksi.Domain.Entities;

namespace Boksi.Application.Schedules.Commands
{
    public class SetEmployeeTimeOffCommand : IRequest<Guid>
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
