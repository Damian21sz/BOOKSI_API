using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IHubContext<Boksi.Api.Hubs.ChatHub> _hubContext;

        public ChatController(IApplicationDbContext dbContext, IHubContext<Boksi.Api.Hubs.ChatHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        [HttpGet("{salonId}/{clientId}")]
        public async Task<IActionResult> GetChatHistory(string salonId, Guid clientId)
        {
            var messages = await _dbContext.ChatMessages
                .Where(m => m.SalonId == salonId && m.ClientId == clientId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("salon/{salonId}")]
        public async Task<IActionResult> GetSalonChats(string salonId)
        {
            // Gets the latest message for each client conversation in the salon
            var chats = await _dbContext.ChatMessages
                .Where(m => m.SalonId == salonId)
                .GroupBy(m => m.ClientId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
                .ToListAsync();

            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return BadRequest("Wiadomość nie może być pusta.");
            }

            if (message.SenderRole != "Client" && message.SenderRole != "Salon")
            {
                return BadRequest("Nieprawidłowa rola nadawcy. Użyj 'Client' lub 'Salon'.");
            }
            
            message.CreatedAt = DateTime.UtcNow;

            _dbContext.ChatMessages.Add(message);
            await _dbContext.SaveChangesAsync(default);

            // Broadcast via SignalR
            // We notify the specific conversation group
            await _hubContext.Clients.Group($"Conversation_{message.SalonId}_{message.ClientId}")
                .SendAsync("ReceiveMessage", message);
                
            // And we notify the Salon group (for the general messages list)
            await _hubContext.Clients.Group($"Salon_{message.SalonId}")
                .SendAsync("NewMessageNotification", message);

            return Ok(message);
        }
    }
}
