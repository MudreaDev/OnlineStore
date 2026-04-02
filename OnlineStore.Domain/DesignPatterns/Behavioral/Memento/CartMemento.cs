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
        public CartMemento(List<CartItem> itemsSnapshot)
        {
            // Constructor for deserialization
            ItemsSnapshot = itemsSnapshot;
        }

        public CartMemento(IEnumerable<CartItem> items)
        {
            // Constructor for creating memento from cart state
            // Create a deep copy of the items to preserve state
            ItemsSnapshot = items.Select(i => new CartItem(i.ProductId, i.Quantity, i.Size, i.Color)).ToList();
        }
    }
}
