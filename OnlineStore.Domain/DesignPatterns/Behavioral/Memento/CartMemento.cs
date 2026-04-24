using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Memento
{
    public class CartMemento
    {
        public List<CartItem> ItemsSnapshot { get; }

        [JsonConstructor]
        public CartMemento(IEnumerable<CartItem> itemsSnapshot)
        {
            // Create a deep copy of the items to preserve state
            ItemsSnapshot = itemsSnapshot.Select(i => new CartItem(i.ProductId, i.Quantity, i.Size, i.Color)).ToList();
        }
    }
}
