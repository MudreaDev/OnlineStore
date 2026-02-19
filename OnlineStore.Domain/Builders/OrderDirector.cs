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

        public Order BuildStandardOrder(User user, List<Product> products)
        {
            return _builder
                .SetUser(user)
                .AddProducts(products)
                .ApplyDiscount(new NoDiscountStrategy())
                .Build();
        }

        public Order BuildPremiumOrder(User user, List<Product> products)
        {
            return _builder
                .SetUser(user)
                .AddProducts(products)
                .ApplyDiscount(new PercentageDiscountStrategy(10)) // 10% discount pentru clienți premium
                .Build();
        }

        public Order BuildSaleOrder(User user, List<Product> products, decimal fixedDiscount)
        {
            return _builder
                .SetUser(user)
                .AddProducts(products)
                .ApplyDiscount(new FixedAmountDiscountStrategy(fixedDiscount))
                .Build();
        }
    }
}
