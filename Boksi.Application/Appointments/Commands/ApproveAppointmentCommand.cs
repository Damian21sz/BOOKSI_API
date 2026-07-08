using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Appointments.Commands
{
    public class ApproveAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
    }

    public class ApproveAppointmentCommandHandler : IRequestHandler<ApproveAppointmentCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ApproveAppointmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ApproveAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { request.AppointmentId }, cancellationToken);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Confirmed;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
