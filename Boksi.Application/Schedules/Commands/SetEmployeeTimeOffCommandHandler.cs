using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Schedules.Commands
{
    public class SetEmployeeTimeOffCommandHandler : IRequestHandler<SetEmployeeTimeOffCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;

        public SetEmployeeTimeOffCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(SetEmployeeTimeOffCommand request, CancellationToken cancellationToken)
        {
            var timeOff = new TimeOff
            {
                EmployeeId = request.EmployeeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,
                Status = TimeOffStatus.Approved // Założenie: jeśli robi to właściciel/ustawienia, jest auto-akceptowane
            };

            _dbContext.TimeOffs.Add(timeOff);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return timeOff.Id;
        }
    }
}
