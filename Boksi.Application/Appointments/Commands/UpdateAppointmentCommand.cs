using Boksi.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Appointments.Commands
{
    public class UpdateAppointmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAppointmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { request.Id }, cancellationToken);
            if (appointment == null) return false;

            appointment.EmployeeId = request.EmployeeId;
            appointment.StartTime = request.StartTime;
            appointment.EndTime = request.EndTime;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
