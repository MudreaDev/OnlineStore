using OnlineStore.Application.Patterns.ChainOfResponsibility;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.Services
{
    public class OrderService
    {
        private readonly IDiscountStrategy _discountStrategy;
        private readonly IOrderHandler _validationChain;

        public OrderService(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;

            // Initialize Chain of Responsibility
            var userHandler = new UserValidationHandler();
            var stockHandler = new StockValidationHandler();
            var pricingHandler = new PricingValidationHandler();

            userHandler.SetNext(stockHandler).SetNext(pricingHandler);
            _validationChain = userHandler;
        }

        public Order PlaceOrder(User user, List<OrderItem> items, string shippingAddress = "N/A", string phoneNumber = "N/A")
        {
            // Use Chain of Responsibility to validate
            string? validationError = _validationChain.Handle(user, items);
            if (validationError != null)
            {
                throw new InvalidOperationException($"Order validation failed: {validationError}");
            }

            decimal total = items.Sum(i => i.UnitPrice * i.Quantity);
            decimal finalTotal = _discountStrategy.ApplyDiscount(total);

            return new Order(user, items, finalTotal, shippingAddress, phoneNumber);
        }
    }
}
