using System;
using System.Collections.Generic;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class ShoppingCart : Entity
    {
        public Guid? UserId { get; set; }
        public Dictionary<Guid, int> ProductIds { get; set; }

        public ShoppingCart(Guid? userId)
        {
            UserId = userId;
            ProductIds = new Dictionary<Guid, int>();
        }

        // Constructor for serialization
        public ShoppingCart()
        {
            ProductIds = new Dictionary<Guid, int>();
        }

        public void AddProduct(Guid productId)
        {
            if (ProductIds.ContainsKey(productId))
            {
                ProductIds[productId]++;
            }
            else
            {
                ProductIds[productId] = 1;
            }
        }

        public void RemoveProduct(Guid productId)
        {
            if (ProductIds.ContainsKey(productId))
            {
                if (ProductIds[productId] > 1)
                {
                    ProductIds[productId]--;
                }
                else
                {
                    ProductIds.Remove(productId);
                }
            }
        }

        public void Clear()
        {
            ProductIds.Clear();
        }
    }
}
