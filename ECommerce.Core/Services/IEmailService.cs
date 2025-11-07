namespace ECommerce.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailServiceAsync(string toEmail, string subject, string htmlBody);
    }
}
