using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Appointments.Commands
{
    public class CreateAppointmentCommand : IRequest<Guid>
    {
        public Guid ClientId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateAppointmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                SalonId = _currentUserService.SalonId ?? "default-salon",
                ClientId = request.ClientId,
                EmployeeId = request.EmployeeId,
                ServiceId = request.ServiceId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                CustomTaskDescription = request.Notes,
                Status = AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}
