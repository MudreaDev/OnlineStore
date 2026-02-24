using System.Collections.Generic;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Strategies;

namespace OnlineStore.Domain.Builders
{
    /// <summary>
    /// Clasa Director pentru Pattern-ul Builder.
    /// Definește configurații predefinite pentru construcția comenzilor.
    /// </summary>
    public class OrderDirector
    {
        private readonly IOrderBuilder _builder;

        public OrderDirector(IOrderBuilder builder)
        {
            _builder = builder;
        }

        public Order BuildStandardOrder(User user, List<OrderItem> items)
        {
            _builder.SetUser(user);
            foreach (var item in items) _builder.AddOrderItem(item);
            
            return _builder
                .ApplyDiscount(new NoDiscountStrategy())
                .Build();
        }

        public Order BuildPremiumOrder(User user, List<OrderItem> items)
        {
            _builder.SetUser(user);
            foreach (var item in items) _builder.AddOrderItem(item);

            return _builder
                .ApplyDiscount(new PercentageDiscountStrategy(10)) // 10% discount pentru clienți premium
                .Build();
        }

        public Order BuildSaleOrder(User user, List<OrderItem> items, decimal fixedDiscount)
        {
            _builder.SetUser(user);
            foreach (var item in items) _builder.AddOrderItem(item);

            return _builder
                .ApplyDiscount(new FixedAmountDiscountStrategy(fixedDiscount))
                .Build();
        }
    }
}
