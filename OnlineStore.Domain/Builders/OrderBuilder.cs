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
        private readonly List<Product> _products = new();
        private IDiscountStrategy _discountStrategy = new NoDiscountStrategy();

        public IOrderBuilder SetUser(User user)
        {
            _user = user;
            return this;
        }

        public IOrderBuilder AddProduct(Product product)
        {
            _products.Add(product);
            return this;
        }

        public IOrderBuilder AddProducts(List<Product> products)
        {
            _products.AddRange(products);
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

            if (!_products.Any())
            {
                throw new InvalidOperationException("Order must contain at least one product.");
            }

            decimal subtotal = _products.Sum(p => p.Price);
            decimal total = _discountStrategy.ApplyDiscount(subtotal);

            return new Order(_user, new List<Product>(_products), total);
        }
    }
}
