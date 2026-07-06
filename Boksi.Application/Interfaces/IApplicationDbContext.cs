using Boksi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<Client> Clients { get; }
        DbSet<ClientNote> ClientNotes { get; }
        DbSet<ClientConsent> ClientConsents { get; }
        DbSet<Appointment> Appointments { get; }
        DbSet<EmployeeSchedule> EmployeeSchedules { get; }
        DbSet<TimeOff> TimeOffs { get; }

        DbSet<Salon> Salons { get; }
        DbSet<BusinessCategory> BusinessCategories { get; }
        DbSet<ServiceCategory> ServiceCategories { get; }
        DbSet<Service> Services { get; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; }
        DbSet<DiscountCode> DiscountCodes { get; }

        DbSet<WaitlistEntry> WaitlistEntries { get; }
        DbSet<GalleryImage> GalleryImages { get; }
        DbSet<FavoriteSalon> FavoriteSalons { get; }
        
        DbSet<AppointmentReview> AppointmentReviews { get; }
        DbSet<ChatMessage> ChatMessages { get; }

        DbSet<LoyaltyProgramSettings> LoyaltyProgramSettings { get; }
        DbSet<ClientLoyaltyCard> ClientLoyaltyCards { get; }
        DbSet<MarketingCampaign> MarketingCampaigns { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
