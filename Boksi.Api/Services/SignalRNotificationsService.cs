using Boksi.Api.Hubs;
using Boksi.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Api.Services
{
    public class SignalRNotificationsService : INotificationsService
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SignalRNotificationsService(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAppointmentCreatedNotificationAsync(string salonId, object notificationData, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(salonId).SendAsync("ReceiveNewAppointment", notificationData, cancellationToken);
        }
    }
}
