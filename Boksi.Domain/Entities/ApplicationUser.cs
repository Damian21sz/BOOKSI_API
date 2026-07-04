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
    }
}
