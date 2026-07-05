using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Services.Commands
{
    public class CreateServiceCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateServiceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DurationMinutes = request.DurationMinutes,
                PhotoUrl = request.PhotoUrl,
                CategoryId = request.CategoryId
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync(cancellationToken);

            return service.Id;
        }
    }
}
