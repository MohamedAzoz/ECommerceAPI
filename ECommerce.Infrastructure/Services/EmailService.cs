using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailServiceAsync(string toEmail, string subject, string htmlBody)
        {
            throw new NotImplementedException();
        }
    }
}
