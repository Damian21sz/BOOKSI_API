using Boksi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Boksi.Application.Interfaces;

namespace Boksi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Salon> Salons { get; set; } = null!;
        public DbSet<BusinessCategory> BusinessCategories { get; set; } = null!;
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<ClientNote> ClientNotes { get; set; } = null!;
        public DbSet<ClientConsent> ClientConsents { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; } = null!;
        public DbSet<TimeOff> TimeOffs { get; set; } = null!;
        
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
        public DbSet<DiscountCode> DiscountCodes { get; set; } = null!;

        public DbSet<WaitlistEntry> WaitlistEntries { get; set; } = null!;
        public DbSet<GalleryImage> GalleryImages { get; set; } = null!;
        public DbSet<FavoriteSalon> FavoriteSalons { get; set; } = null!;
        
        public DbSet<AppointmentReview> AppointmentReviews { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        public DbSet<LoyaltyProgramSettings> LoyaltyProgramSettings { get; set; } = null!;
        public DbSet<ClientLoyaltyCard> ClientLoyaltyCards { get; set; } = null!;
        public DbSet<MarketingCampaign> MarketingCampaigns { get; set; } = null!;

        public DbSet<Changelog> Changelogs { get; set; } = null!;
        public DbSet<ChangelogReadReceipt> ChangelogReadReceipts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Salon Entities with Global Query Filters
            modelBuilder.Entity<ServiceCategory>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<Service>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<Employee>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<Appointment>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<EmployeeSchedule>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<TimeOff>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<WaitlistEntry>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<GalleryImage>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<AppointmentReview>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<ChatMessage>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<LoyaltyProgramSettings>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<ClientLoyaltyCard>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<MarketingCampaign>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<ClientNote>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);
            modelBuilder.Entity<ClientConsent>().HasQueryFilter(e => _currentUserService.SalonId == null || e.SalonId == _currentUserService.SalonId);

            // Additional configurations if needed
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<SubscriptionPlan>()
                .Property(p => p.PricePerMonth)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<DiscountCode>()
                .Property(d => d.Value)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<Salon>()
                .Property(s => s.BaseSubscriptionPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
