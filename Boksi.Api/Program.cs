using Boksi.Infrastructure.Data;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<Boksi.Application.Interfaces.IEmailService, Boksi.Infrastructure.Services.DummyEmailService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Boksi.Application.Class1).Assembly));

// Configure CORS for the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure Finbuckle MultiTenant
builder.Services.AddMultiTenant<TenantInfo>()
    .WithHeaderStrategy("X-Tenant-Id")
    .WithConfigurationStore();

// Configure Entity Framework Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<Boksi.Application.Interfaces.IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

// Configure Identity
builder.Services.AddIdentity<Boksi.Domain.Entities.ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
});
// Note: You would normally add .AddJwtBearer(...) here if you are using JWT.

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

// Finbuckle middleware must be before routing/auth
app.UseMultiTenant();

app.UseAuthorization();
app.MapControllers();

app.Run();
