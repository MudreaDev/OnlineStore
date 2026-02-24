using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Strategies;

namespace OnlineStore.Domain.Builders
{
    /// <summary>
    /// Implementare concretă a IOrderBuilder.
    /// Incapsulează logica de construcție a unui Order.
    /// </summary>
    public class OrderBuilder : IOrderBuilder
    {
        private User? _user;
        private readonly List<OrderItem> _items = new();
        private IDiscountStrategy _discountStrategy = new NoDiscountStrategy();

        public IOrderBuilder SetUser(User user)
        {
            _user = user;
            return this;
        }

        public IOrderBuilder AddOrderItem(OrderItem item)
        {
            _items.Add(item);
            return this;
        }

        public IOrderBuilder AddProduct(Product product, int quantity = 1, string? size = null, string? color = null)
        {
            var item = new OrderItem(product.Id, product.Name, product.Price, quantity, size, color);
            _items.Add(item);
            return this;
        }

        public IOrderBuilder ApplyDiscount(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
            return this;
        }

        public Order Build()
        {
            if (_user == null)
            {
                throw new InvalidOperationException("User must be set before building an order.");
            }

            if (!_items.Any())
            {
                throw new InvalidOperationException("Order must contain at least one product.");
            }

            decimal subtotal = _items.Sum(i => i.UnitPrice * i.Quantity);
            decimal total = _discountStrategy.ApplyDiscount(subtotal);

            return new Order(_user, new List<OrderItem>(_items), total);
        }
    }
}
