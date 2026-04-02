using OnlineStore.Domain.Interfaces;
using System;

namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    // The Concrete Component provides default implementation of operations
    public class EmailNotification : INotification
    {
        private readonly IEmailService? _emailService;
        private readonly string? _to;

        public EmailNotification() { }

        public EmailNotification(IEmailService emailService, string to)
        {
            _emailService = emailService;
            _to = to;
        }

        public string Send(string message)
        {
            if (_emailService != null && !string.IsNullOrEmpty(_to))
            {
                // Chemăm serviciul REAL de email
                _emailService.SendEmailAsync(_to, "Notificare Magazin", message).GetAwaiter().GetResult();
                return $"[REAL Email] Sent to {_to}: {message}";
            }
            
            return $"[Email Simulation] Content: {message}";
        }
    }
}
