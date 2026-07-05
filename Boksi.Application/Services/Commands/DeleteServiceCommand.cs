using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Services.Commands
{
    public class DeleteServiceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteServiceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (service == null) return false;

            service.IsDeleted = true; // Soft Delete
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
