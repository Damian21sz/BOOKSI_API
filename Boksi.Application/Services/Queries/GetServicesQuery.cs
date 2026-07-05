using Boksi.Application.DTOs;
using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Services.Queries
{
    public class GetServicesQuery : IRequest<List<ServiceDto>>
    {
    }

    public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, List<ServiceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetServicesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => !s.IsDeleted)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes,
                    PhotoUrl = s.PhotoUrl,
                    CategoryId = s.CategoryId,
                    CategoryName = s.Category != null ? s.Category.Name : null
                })
                .ToListAsync(cancellationToken);
        }
    }
}
