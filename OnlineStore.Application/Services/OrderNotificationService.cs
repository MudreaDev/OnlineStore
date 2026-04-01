using System.Collections.Generic;
using OnlineStore.Domain.DesignPatterns.Behavioral.Observer;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Services
{
    public class OrderNotificationService
    {
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        public OrderNotificationService(OnlineStore.Domain.Interfaces.IEmailService emailService)
        {
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

        public void Notify(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.Update(order);
            }
        }
    }
}
