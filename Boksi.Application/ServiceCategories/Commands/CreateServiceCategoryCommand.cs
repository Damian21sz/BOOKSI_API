using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.ServiceCategories.Commands
{
    public class CreateServiceCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateServiceCategoryCommandHandler : IRequestHandler<CreateServiceCategoryCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateServiceCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateServiceCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new ServiceCategory
            {
                Id = Guid.NewGuid(),
                SalonId = _currentUserService.SalonId ?? "default-salon",
                Name = request.Name,
                Description = request.Description
            };

            _context.ServiceCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
