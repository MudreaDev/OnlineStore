using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Memento
{
    public class CartMemento
    {
        public List<CartItem> ItemsSnapshot { get; }

        public CartMemento(List<CartItem> items)
        {
            // Create a deep copy of the items to preserve state
            ItemsSnapshot = items.Select(i => new CartItem(i.ProductId, i.Quantity, i.Size, i.Color)).ToList();
        }
    }
}
