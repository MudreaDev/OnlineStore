using System;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Observer
{
    public class DashboardObserver : IOrderObserver
    {
        public void Update(Order order)
        {
            // Simulate updating an admin dashboard
            Console.WriteLine($"[DashboardObserver] Dashboard has been updated. Order {order.Id} is {order.Status}.");
        }
    }
}
