using Boksi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalonsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;

        public SalonsController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalons(
            [FromQuery] string? service,
            [FromQuery] string? location,
            [FromQuery] Guid? categoryId,
            [FromQuery] double? lat,
            [FromQuery] double? lon,
            [FromQuery] string? sortBy = "distance",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6)
        {
            var query = _dbContext.Salons.Include(s => s.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(s => s.BusinessCategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(service))
            {
                var serviceLower = service.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(serviceLower) || 
                                         (s.Description != null && s.Description.ToLower().Contains(serviceLower)) ||
                                         (s.Category != null && s.Category.Name.ToLower().Contains(serviceLower)));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var locationLower = location.ToLower();
                query = query.Where(s => s.Address != null && s.Address.ToLower().Contains(locationLower));
            }

            var salons = await query.ToListAsync();

            // Calculate distance if lat/lon provided
            var resultList = salons.Select(s => new SalonDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address ?? "Brak adresu",
                Rating = 5.0, // Mocked rating for MVP
                ReviewsCount = 15,
                PhotoUrl = null, // Will use placeholder on front
                Distance = lat.HasValue && lon.HasValue && s.Latitude.HasValue && s.Longitude.HasValue
                    ? CalculateDistance(lat.Value, lon.Value, s.Latitude.Value, s.Longitude.Value)
                    : (double?)null
            }).ToList();

            // Sorting
            if (sortBy == "name")
            {
                resultList = resultList.OrderBy(s => s.Name).ToList();
            }
            else if (sortBy == "rating")
            {
                resultList = resultList.OrderByDescending(s => s.Rating).ToList();
            }
            else // default "distance"
            {
                resultList = resultList.OrderBy(s => s.Distance ?? double.MaxValue).ToList();
            }

            // Pagination
            var totalCount = resultList.Count;
            var items = resultList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  
            var dLon = Deg2Rad(lon2 - lon1); 
            var a = 
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * 
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2); 
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
            var d = R * c; // Distance in km
            return Math.Round(d, 1);
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }

    public class SalonDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string? PhotoUrl { get; set; }
        public double? Distance { get; set; }
    }
}
