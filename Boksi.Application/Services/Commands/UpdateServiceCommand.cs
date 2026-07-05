using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Services.Commands
{
    public class UpdateServiceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public Guid CategoryId { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateServiceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (service == null) return false;

            service.Name = request.Name;
            service.Description = request.Description;
            service.Price = request.Price;
            service.DurationMinutes = request.DurationMinutes;
            service.CategoryId = request.CategoryId;
            service.PhotoUrl = request.PhotoUrl;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
