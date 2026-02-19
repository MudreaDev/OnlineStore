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
        IOrderBuilder AddProduct(Product product);
        IOrderBuilder AddProducts(List<Product> products);
        IOrderBuilder ApplyDiscount(IDiscountStrategy discountStrategy);
        Order Build();
    }
}
