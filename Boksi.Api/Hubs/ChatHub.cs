using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Boksi.Api.Hubs
{
    public class ChatHub : Hub
    {
        // Klienci (salon/klient końcowy) dołączają do grupy nazwanej identyfikatorem salonu i/lub klienta,
        // by otrzymywać tylko wiadomości ich dotyczące.
        
        public async Task JoinSalonGroup(string salonId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Salon_{salonId}");
        }

        public async Task LeaveSalonGroup(string salonId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Salon_{salonId}");
        }

        public async Task JoinClientGroup(string clientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Client_{clientId}");
        }

        public async Task LeaveClientGroup(string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Client_{clientId}");
        }

        public async Task JoinConversation(string salonId, string clientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{salonId}_{clientId}");
        }

        public async Task LeaveConversation(string salonId, string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{salonId}_{clientId}");
        }
    }
}
