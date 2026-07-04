using Boksi.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Boksi.Infrastructure.Services
{
    public class DummyEmailService : IEmailService
    {
        private readonly ILogger<DummyEmailService> _logger;

        public DummyEmailService(ILogger<DummyEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("--- SENDING EMAIL ---");
            _logger.LogInformation("To: {To}", to);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);
            _logger.LogInformation("-----------------------");

            return Task.CompletedTask;
        }
    }
}
