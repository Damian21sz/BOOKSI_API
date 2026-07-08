using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Boksi.Api.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task JoinSalonGroup(string salonId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, salonId);
        }

        public async Task LeaveSalonGroup(string salonId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, salonId);
        }
    }
}
