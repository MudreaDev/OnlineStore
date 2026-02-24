using System;

namespace OnlineStore.Domain.Entities
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        public CartItem() { }

        public CartItem(Guid productId, int quantity, string? size = null, string? color = null)
        {
            ProductId = productId;
            Quantity = quantity;
            Size = size;
            Color = color;
        }
    }
}
