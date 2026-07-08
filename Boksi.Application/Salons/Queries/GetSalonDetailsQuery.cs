using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Salons.Queries
{
    public class GetSalonDetailsQuery : IRequest<SalonDetailsDto?>
    {
        public Guid SalonId { get; set; }
    }

    public class GetSalonDetailsQueryHandler : IRequestHandler<GetSalonDetailsQuery, SalonDetailsDto?>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetSalonDetailsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SalonDetailsDto?> Handle(GetSalonDetailsQuery request, CancellationToken cancellationToken)
        {
            // Omijamy global filters dla TenantId, ponieważ to zapytanie publiczne
            var salon = await _dbContext.Salons
                .IgnoreQueryFilters()
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == request.SalonId, cancellationToken);

            if (salon == null) return null;

            var tenantId = salon.Identifier;

            // Pobranie kategorii usług z przypisanymi usługami
            var categories = await _dbContext.ServiceCategories
                .IgnoreQueryFilters()
                .Where(c => c.SalonId == tenantId)
                .Select(c => new ServiceCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = "#000000",
                    Services = _dbContext.Services
                        .IgnoreQueryFilters()
                        .Where(s => s.CategoryId == c.Id && s.SalonId == tenantId)
                        .Select(s => new ServiceDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Description = s.Description,
                            Price = s.Price,
                            DurationMinutes = s.DurationMinutes,
                            PhotoUrl = s.PhotoUrl
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            // Filtrujemy kategorie, żeby nie pokazywać pustych
            categories = categories.Where(c => c.Services.Any()).ToList();

            // Pobranie pracowników
            var employees = await _dbContext.Employees
                .IgnoreQueryFilters()
                .Where(e => e.SalonId == tenantId && e.IsActive)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    PhotoUrl = null // Tymczasowo brak zdjęcia pracownika w domenie, można zaimplementować później
                })
                .ToListAsync(cancellationToken);

            return new SalonDetailsDto
            {
                Id = salon.Id,
                Name = salon.Name,
                Description = salon.Description,
                Address = salon.Address ?? "Brak adresu",
                PhoneNumber = salon.PhoneNumber,
                CategoryName = salon.Category?.Name,
                Categories = categories,
                Employees = employees,
                Rating = 5.0, // Mocked
                ReviewsCount = 15 // Mocked
            };
        }
    }

    public class SalonDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Address { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? CategoryName { get; set; }
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public List<ServiceCategoryDto> Categories { get; set; } = new();
        public List<EmployeeDto> Employees { get; set; } = new();
    }

    public class ServiceCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public List<ServiceDto> Services { get; set; } = new();
    }

    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
