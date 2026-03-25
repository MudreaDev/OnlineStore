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

        public async Task SendPasswordResetCodeAsync(string email, string code)
        {
            string subject = "Cod de resetare parolă";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2>Resetare parolă</h2>
                    <p>Ai solicitat resetarea parolei pentru contul tău. Folosește codul de mai jos pentru a continua:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; font-size: 24px; font-weight: bold; text-align: center; letter-spacing: 5px; border-radius: 5px;'>
                        {code}
                    </div>
                    <p>Acest cod este valabil timp de <strong>3 minute</strong>. Dacă nu ai solicitat tu această resetare, te rugăm să ignori acest email.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
