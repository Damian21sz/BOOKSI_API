using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Boksi.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Use a dummy connection string for design-time
            var connectionString = "Host=localhost;Database=BoksiDummyDb;Username=postgres;Password=postgres";

            builder.UseNpgsql(connectionString);

            var tenantInfo = new TenantInfo { Id = "dummy-tenant-id", Identifier = "dummy-tenant" };

            return new ApplicationDbContext(tenantInfo, builder.Options);
        }

    }
}
