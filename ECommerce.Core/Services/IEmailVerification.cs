namespace ECommerce.Core.Services
{
    public interface IEmailVerification
    {
        Task SendEmailVerificationAsync(string toEmail, string subject, string htmlBody);
    }
}
