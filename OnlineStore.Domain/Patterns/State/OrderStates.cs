using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using System;

namespace OnlineStore.Domain.Patterns.State
{
    public class PendingState : IOrderState
    {
        public void Pay(Order order)
        {
            Console.WriteLine("Payment successful. Transitioning to Paid state.");
            order.SetState(new PaidState());
            order.Status = OrderStatus.Paid;
        }

        public void Ship(Order order)
        {
            throw new InvalidOperationException("Cannot ship a pending order. Payment is required.");
        }

        public void Cancel(Order order)
        {
            Console.WriteLine("Order cancelled.");
            order.SetState(new CancelledState());
            order.Status = OrderStatus.Cancelled;
        }

        public string GetStatusName() => "Pending";
    }

    public class PaidState : IOrderState
    {
        public void Pay(Order order)
        {
            throw new InvalidOperationException("Order is already paid.");
        }

        public void Ship(Order order)
        {
            Console.WriteLine("Order shipped. Transitioning to Shipped state.");
            order.SetState(new ShippedState());
            order.Status = OrderStatus.Shipped;
        }

        public void Cancel(Order order)
        {
            Console.WriteLine("Order cancelled and payment refunded.");
            order.SetState(new CancelledState());
            order.Status = OrderStatus.Cancelled;
        }

        public string GetStatusName() => "Paid";
    }

    public class ShippedState : IOrderState
    {
        public void Pay(Order order) => throw new InvalidOperationException("Order is already paid.");
        
        public void Ship(Order order) => throw new InvalidOperationException("Order is already shipped.");

        public void Cancel(Order order)
        {
            throw new InvalidOperationException("Cannot cancel an order that has already been shipped.");
        }

        public string GetStatusName() => "Shipped";
    }

    public class CancelledState : IOrderState
    {
        public void Pay(Order order) => throw new InvalidOperationException("Cannot pay for a cancelled order.");
        public void Ship(Order order) => throw new InvalidOperationException("Cannot ship a cancelled order.");
        public void Cancel(Order order) => throw new InvalidOperationException("Order is already cancelled.");

        public string GetStatusName() => "Cancelled";
    }
}
