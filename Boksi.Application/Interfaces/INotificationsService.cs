using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Interfaces
{
    public interface INotificationsService
    {
        Task SendAppointmentCreatedNotificationAsync(string salonId, object notificationData, CancellationToken cancellationToken);
    }
}
