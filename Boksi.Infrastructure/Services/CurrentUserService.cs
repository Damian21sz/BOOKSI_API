using System.Security.Claims;
using Boksi.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Boksi.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // For now, if the user has a SalonId claim, we use it. Or we can read it from a custom header 'X-Salon-Id'
        // Let's support both: first check Claim, then fallback to Header.
        public string? SalonId 
        {
            get 
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return null;

                var claimSalonId = context.User?.FindFirstValue("SalonId");
                if (!string.IsNullOrEmpty(claimSalonId)) return claimSalonId;

                if (context.Request.Headers.TryGetValue("X-Salon-Id", out var headerSalonId))
                {
                    return headerSalonId.ToString();
                }

                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenantId))
                {
                    return headerTenantId.ToString();
                }

                return null;
            }
        }
    }
}
