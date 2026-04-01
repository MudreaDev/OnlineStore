using System;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Observer
{
    public class SmsObserver : IOrderObserver
    {
        public void Update(Order order)
        {
            // Simulate sending SMS
            Console.WriteLine($"[SmsObserver] Sent SMS: Your order {order.Id} is now {order.Status}.");
        }
    }
}
