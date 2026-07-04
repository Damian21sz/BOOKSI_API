using Boksi.Domain.Entities;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Boksi.Application.Interfaces;

namespace Boksi.Infrastructure.Data
{
    public class ApplicationDbContext : MultiTenantIdentityDbContext<ApplicationUser, IdentityRole, string>, IApplicationDbContext
    {
        public ApplicationDbContext(ITenantInfo? tenantInfo, DbContextOptions<ApplicationDbContext> options) : base(tenantInfo ?? new TenantInfo(), options)
        {
        }

        public DbSet<Salon> Salons { get; set; } = null!;
        public DbSet<BusinessCategory> BusinessCategories { get; set; } = null!;
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tenant Entities
            modelBuilder.Entity<ServiceCategory>().IsMultiTenant();
            modelBuilder.Entity<Service>().IsMultiTenant();
            modelBuilder.Entity<Employee>().IsMultiTenant();
            modelBuilder.Entity<Appointment>().IsMultiTenant();
            modelBuilder.Entity<EmployeeSchedule>().IsMultiTenant();
            modelBuilder.Entity<TimeOff>().IsMultiTenant();
            modelBuilder.Entity<WaitlistEntry>().IsMultiTenant();
            modelBuilder.Entity<GalleryImage>().IsMultiTenant();
            modelBuilder.Entity<AppointmentReview>().IsMultiTenant();
            modelBuilder.Entity<ChatMessage>().IsMultiTenant();
            modelBuilder.Entity<LoyaltyProgramSettings>().IsMultiTenant();
            modelBuilder.Entity<ClientLoyaltyCard>().IsMultiTenant();
            modelBuilder.Entity<MarketingCampaign>().IsMultiTenant();

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
