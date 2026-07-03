using Boksi.Domain.Entities;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boksi.Infrastructure.Data
{
    public class ApplicationDbContext : MultiTenantDbContext
    {
        public ApplicationDbContext(ITenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        public ApplicationDbContext(ITenantInfo tenantInfo, DbContextOptions<ApplicationDbContext> options) : base(tenantInfo, options)
        {
        }

        public DbSet<Salon> Salons { get; set; } = null!;
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tenant Entities
            modelBuilder.Entity<ServiceCategory>().IsMultiTenant();
            modelBuilder.Entity<Service>().IsMultiTenant();
            modelBuilder.Entity<Employee>().IsMultiTenant();
            modelBuilder.Entity<Appointment>().IsMultiTenant();

            // Additional configurations if needed
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
