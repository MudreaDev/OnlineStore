using System;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.DesignPatterns.Structural.Decorator;

namespace OnlineStore.Application.Services
{
    /// <summary>
    /// Serviciu pentru gestionarea notificărilor de disponibilitate (Pattern Observer).
    /// Când un produs revine în stoc (0 -> 1+), toți abonații sunt notificați.
    /// </summary>
    public class ProductAvailabilityService
    {
        private readonly IEmailService _emailService;

        public ProductAvailabilityService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void NotifyBackInStock(Product product)
        {
            if (product.Stock > 0 && product.SubscriberEmails.Any())
            {
                Console.WriteLine($"[Notify] Produsul '{product.Name}' a revenit în stoc. Notificăm {product.SubscriberEmails.Count} utilizatori.");
                
                foreach (var email in product.SubscriberEmails.ToList())
                {
                    // Utilizăm sistemul tău de notificări (Decorator Pattern)
                    // Mesajul este real (trimis pe email) și decorat (cu loguri pentru SMS/Push, chiar dacă sunt simulate)
                    var message = $"Produsul '{product.Name}' a revenit în stoc! Preț: {product.Price}$. Grăbește-te, stocul este limitat!";
                    
                    // Creăm lanțul de decoratori conform cerințelor tale de la Lab 2
                    INotification notification = new EmailNotification(_emailService, email);
                    notification = new SmsDecorator(notification);
                    notification = new PushDecorator(notification);

                    var result = notification.Send(message);
                    Console.WriteLine($"[AvailabilityService] {result}");
                }

                // După notificare, curățăm lista de abonați
                product.SubscriberEmails.Clear();
            }
        }
    }
}
