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
            if (order.User == null || string.IsNullOrEmpty(order.User.Email)) return;

            _emailService.SendEmailAsync(
                order.User.Email,
                $"Status Comandă Actualizat - {order.Id.ToString()[..8]}",
                $"Bună {order.User.Username}, statusul comenzii tale a fost actualizat la: {order.Status}."
            ).GetAwaiter().GetResult();
            
            Console.WriteLine($"[EmailObserver] Sent email notification for Order {order.Id} regarding status {order.Status}");
        }
    }
}
