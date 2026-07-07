using Microsoft.AspNetCore.Identity;

namespace Boksi.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? FirstLoginDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public bool MustChangePassword { get; set; } = false;
        
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? CoverageRadiusKm { get; set; }
        
        public string? SalonId { get; set; }
    }
}
