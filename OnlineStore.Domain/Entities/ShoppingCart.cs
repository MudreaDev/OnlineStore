using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class ShoppingCart : Entity
    {
        public User User { get; set; }
        public List<Product> Products { get; set; }

        public ShoppingCart(User user)
        {
            User = user;
            Products = new List<Product>();
        }

        public void AddProduct(Product product)
        {
            Products.Add(product);
        }

        public void RemoveProduct(Guid productId)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                Products.Remove(product);
            }
        }

        public void Clear()
        {
            Products.Clear();
        }

        public decimal CalculateTotal()
        {
            return Products.Sum(p => p.Price);
        }
    }
}
