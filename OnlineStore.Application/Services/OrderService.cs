using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.Services
{
    // DIP: Dependency Inversion Principle (depends on interfaces, not implementations)
    // SRP: Single Responsibility (handles order processing)
    //Am mutat logica de calcul într-un serviciu separat pentru a respecta SRP.
    //Serviciul depinde de o interfață, nu de o implementare concretă.”


    public class OrderService
    {
        private readonly IDiscountStrategy _discountStrategy;//depinde doar de Interfata nu de alta clasa, DIP

        public OrderService(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
        }


        public decimal CalculateTotal(List<Product> products)
        {
            decimal total = products.Sum(p => p.Price);
            return _discountStrategy.ApplyDiscount(total);
        }
    }
}
