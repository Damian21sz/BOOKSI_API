using Boksi.Application.DTOs;
using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.ServiceCategories.Queries
{
    public class GetServiceCategoriesQuery : IRequest<List<ServiceCategoryDto>>
    {
    }

    public class GetServiceCategoriesQueryHandler : IRequestHandler<GetServiceCategoriesQuery, List<ServiceCategoryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetServiceCategoriesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceCategoryDto>> Handle(GetServiceCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceCategories
                .Select(c => new ServiceCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync(cancellationToken);
        }
    }
}
