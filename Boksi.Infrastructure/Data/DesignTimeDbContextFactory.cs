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
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(basePath, "..", "Boksi.Api");
            
            if (!Directory.Exists(apiPath))
            {
                apiPath = Path.Combine(basePath, "Boksi.Api");
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            var tenantInfo = new TenantInfo();
            configuration.GetSection("Finbuckle:MultiTenant:Stores:ConfigurationStore:Tenants:0").Bind(tenantInfo);
            
            // Fallback if no tenant is found in configuration
            if (string.IsNullOrEmpty(tenantInfo.Id))
            {
                tenantInfo.Id = "default";
                tenantInfo.Identifier = "default";
            }

            return new ApplicationDbContext(tenantInfo, builder.Options);
        }

    }
}
