using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.ServiceCategories.Commands
{
    public class UpdateServiceCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateServiceCategoryCommandHandler : IRequestHandler<UpdateServiceCategoryCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateServiceCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateServiceCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
