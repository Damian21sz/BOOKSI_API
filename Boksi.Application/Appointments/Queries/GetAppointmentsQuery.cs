using Boksi.Application.DTOs;
using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Appointments.Queries
{
    public class GetAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
    }

    public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAppointmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ClientId = a.ClientId,
                    ClientName = a.Client != null ? a.Client.FirstName + " " + a.Client.LastName : "Klient usunięty",
                    ClientPhone = a.Client != null ? a.Client.PhoneNumber : null,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee != null ? a.Employee.FirstName + " " + a.Employee.LastName : "Brak pracownika",
                    ServiceId = a.ServiceId ?? System.Guid.Empty,
                    ServiceName = a.Service != null ? a.Service.Name : "Brak",
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = (int)a.Status,
                    Notes = a.CustomTaskDescription
                })
                .ToListAsync(cancellationToken);
        }
    }
}
