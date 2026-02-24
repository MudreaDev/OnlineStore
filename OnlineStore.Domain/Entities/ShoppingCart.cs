using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class ShoppingCart : Entity
    {
        public Guid? UserId { get; set; }
        public List<CartItem> Items { get; set; }

        public ShoppingCart(Guid? userId)
        {
            UserId = userId;
            Items = new List<CartItem>();
        }

        public ShoppingCart()
        {
            Items = new List<CartItem>();
        }

        public void AddProduct(Guid productId, string? size = null, string? color = null)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size && i.Color == color);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                Items.Add(new CartItem(productId, 1, size, color));
            }
        }

        public void RemoveProduct(Guid productId, string? size = null, string? color = null)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size && i.Color == color);
            if (existingItem != null)
            {
                if (existingItem.Quantity > 1)
                {
                    existingItem.Quantity--;
                }
                else
                {
                    Items.Remove(existingItem);
                }
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
