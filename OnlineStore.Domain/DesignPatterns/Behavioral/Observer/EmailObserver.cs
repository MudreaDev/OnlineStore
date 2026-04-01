using System;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Observer
{
    public class EmailObserver : IOrderObserver
    {
        private readonly IEmailService _emailService;

        public EmailObserver(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Update(Order order)
        {
            _emailService.SendEmailAsync(
                "customer@example.com", // in real world, this would be user.Email
                $"Status Comandă Actualizat - {order.Id}",
                $"Statusul comenzii tale a fost actualizat la: {order.Status}."
            ).GetAwaiter().GetResult();
            
            Console.WriteLine($"[EmailObserver] Sent email notification for Order {order.Id} regarding status {order.Status}");
        }
    }
}
