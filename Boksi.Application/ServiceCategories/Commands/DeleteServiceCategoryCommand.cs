using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.ServiceCategories.Commands
{
    public class DeleteServiceCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteServiceCategoryCommandHandler : IRequestHandler<DeleteServiceCategoryCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteServiceCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteServiceCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null) return false;

            category.IsDeleted = true; // Soft Delete
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
