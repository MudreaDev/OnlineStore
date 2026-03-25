using Microsoft.Extensions.Configuration;
using OnlineStore.Domain.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendOrderConfirmationAsync(string userEmail, string orderId, decimal totalAmount)
        {
            string subject = $"Confirmare Comandă #{orderId}";
            string body = $@"
                <h1>Vă mulțumim pentru comandă!</h1>
                <p>Comanda dumneavoastră cu ID-ul <strong>{orderId}</strong> a fost înregistrată cu succes.</p>
                <p>Total de plată: <strong>{totalAmount} RON</strong></p>
                <p>Vom reveni cu detalii despre livrare în cel mai scurt timp.</p>";

            await SendEmailAsync(userEmail, subject, body);
        }
    }
}
