using System.Collections.Generic;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Interfaces
{
    /// <summary>
    /// Interfață pentru Pattern-ul Builder.
    /// Scop: Permite construcția pas cu pas a unui obiect complex (Order).
    /// </summary>
    public interface IOrderBuilder
    {
        IOrderBuilder SetUser(User user);
        IOrderBuilder AddOrderItem(OrderItem item);
        IOrderBuilder AddProduct(Product product, int quantity = 1, string? size = null, string? color = null);
        IOrderBuilder ApplyDiscount(IDiscountStrategy discountStrategy);
        Order Build();
    }
}
