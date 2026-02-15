using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.Services
{
    public class OrderService
    {
        private readonly IDiscountStrategy _discountStrategy;

        public OrderService(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
        }

        public Order PlaceOrder(User user, List<Product> products)
        {
            decimal total = products.Sum(p => p.Price);
            decimal finalTotal = _discountStrategy.ApplyDiscount(total);

            return new Order(user, products, finalTotal);
        }
    }
}
