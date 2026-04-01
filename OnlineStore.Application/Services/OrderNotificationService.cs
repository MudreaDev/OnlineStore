using System.Collections.Generic;
using OnlineStore.Domain.DesignPatterns.Behavioral.Observer;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Services
{
    /// <summary>
    /// OrderSubject (Observer Pattern) — joacă rolul de Subject conform GoF.
    /// Menține lista de observatori și îi notifică automat când starea unei comenzi se schimbă.
    /// Separat de entitatea Order pentru a nu complica serializarea EF Core.
    /// </summary>
    public class OrderNotificationService
    {
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        public OrderNotificationService(OnlineStore.Domain.Interfaces.IEmailService emailService)
        {
            // Attach default observers
            Attach(new EmailObserver(emailService));
            Attach(new SmsObserver());
            Attach(new DashboardObserver());
        }

        public void Attach(IOrderObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IOrderObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// Actualizează statusul comenzii și notifică automat toți observatorii.
        /// Aceasta respectă pattern-ul Observer: subiectul notifică la schimbarea stării.
        /// </summary>
        public void UpdateStatusAndNotify(Order order, Domain.Enums.OrderStatus newStatus)
        {
            order.Status = newStatus;
            Notify(order);
        }

        private void Notify(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.Update(order);
            }
        }
    }
}
