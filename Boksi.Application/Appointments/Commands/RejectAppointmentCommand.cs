using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Appointments.Commands
{
    public class RejectAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
    }

    public class RejectAppointmentCommandHandler : IRequestHandler<RejectAppointmentCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public RejectAppointmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RejectAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { request.AppointmentId }, cancellationToken);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
