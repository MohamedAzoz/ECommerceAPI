using ECommerce.Core.Services;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Infrastructure.Services
{
    public class EmailVerification : IEmailVerification
    {
        public async Task SendEmailVerificationAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpclient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tlmbalrby798@gmail.com", "feor anlb ihtr krrv"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage()
            {
                From = new MailAddress("tlmbalrby798@gmail.com", "E-Commerce"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpclient.SendMailAsync(mailMessage);
        }

    }
}
