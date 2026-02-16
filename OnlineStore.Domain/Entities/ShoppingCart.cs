using System;
using System.Collections.Generic;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class ShoppingCart : Entity
    {
        public Guid? UserId { get; set; }
        public List<Guid> ProductIds { get; set; }

        public ShoppingCart(Guid? userId)
        {
            UserId = userId;
            ProductIds = new List<Guid>();
        }

        // Constructor for serialization
        public ShoppingCart()
        {
            ProductIds = new List<Guid>();
        }

        public void AddProduct(Guid productId)
        {
            ProductIds.Add(productId);
        }

        public void RemoveProduct(Guid productId)
        {
            ProductIds.Remove(productId);
        }

        public void Clear()
        {
            ProductIds.Clear();
        }
    }
}
