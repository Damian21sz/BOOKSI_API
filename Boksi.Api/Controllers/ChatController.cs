using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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

        public ChatController(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetChatHistory(Guid clientId)
        {
            var messages = await _dbContext.ChatMessages
                .Where(m => m.ClientId == clientId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
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

            _dbContext.ChatMessages.Add(message);
            await _dbContext.SaveChangesAsync(default);

            // Tutaj docelowo dodamy wywołanie SignalR: 
            // _hubContext.Clients.Group(message.ClientId.ToString()).SendAsync("ReceiveMessage", message);

            return Ok(message);
        }
    }
}
