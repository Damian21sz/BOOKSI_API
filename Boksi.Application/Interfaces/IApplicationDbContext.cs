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
        DbSet<Appointment> Appointments { get; }
        DbSet<EmployeeSchedule> EmployeeSchedules { get; }
        DbSet<TimeOff> TimeOffs { get; }

        DbSet<Salon> Salons { get; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; }
        DbSet<DiscountCode> DiscountCodes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
