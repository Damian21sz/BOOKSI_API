using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Boksi.Infrastructure.Data; // Warning: using DbContext directly in Middleware for performance/simplicity
using Microsoft.Extensions.DependencyInjection;
using Boksi.Domain.Entities;
using Boksi.Application.Interfaces;

namespace Boksi.Api.Middlewares
{
    public class SubscriptionRequirementMiddleware
    {
        private readonly RequestDelegate _next;

        public SubscriptionRequirementMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip admin and public endpoints
            var path = context.Request.Path.Value;
            if (path != null && (path.StartsWith("/api/admin") || path.StartsWith("/api/public") || path.Contains("/auth/")))
            {
                await _next(context);
                return;
            }

            // Global Admins bypass subscription requirements
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated && context.User.IsInRole("GlobalAdmin"))
            {
                await _next(context);
                return;
            }

            using var scope = context.RequestServices.CreateScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
            var salonId = currentUserService.SalonId;

            if (!string.IsNullOrEmpty(salonId))
            {
                // We have a tenant context, meaning someone is accessing a salon's data
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var salon = await dbContext.Salons.FirstOrDefaultAsync(s => s.Id.ToString() == salonId || s.Identifier == salonId);
                if (salon != null)
                {
                    if (salon.SubscriptionValidUntil < DateTime.UtcNow)
                    {
                        if (salon.SubscriptionStatus != SubscriptionStatus.Expired)
                        {
                            salon.SubscriptionStatus = SubscriptionStatus.Expired;
                            await dbContext.SaveChangesAsync();
                        }

                        // Allow access to subscription renewal endpoints even if expired
                        if (path != null && !path.StartsWith("/api/subscriptions/renew"))
                        {
                            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"error\": \"Subscription expired. Please renew your subscription to continue using the platform.\"}");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
