using OnlineStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.Patterns.ChainOfResponsibility
{
    public class UserValidationHandler : BaseOrderHandler
    {
        public override string? Handle(User user, List<OrderItem> items)
        {
            if (user == null)
            {
                return "User cannot be null.";
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return "User must have a valid email.";
            }

            return base.Handle(user, items);
        }
    }

    public class StockValidationHandler : BaseOrderHandler
    {
        public override string? Handle(User user, List<OrderItem> items)
        {
            if (items == null || !items.Any())
            {
                return "Order must contain at least one item.";
            }

            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                {
                    return $"Invalid quantity for item {item.ProductId}.";
                }
                
                // Note: Real implementation would check against Product repository stock
                // For this pattern demonstration, we validate quantity logic.
            }

            return base.Handle(user, items);
        }
    }

    public class PricingValidationHandler : BaseOrderHandler
    {
        public override string? Handle(User user, List<OrderItem> items)
        {
            foreach (var item in items)
            {
                if (item.UnitPrice < 0)
                {
                    return $"Invalid price for item {item.ProductId}.";
                }
            }

            return base.Handle(user, items);
        }
    }
}
