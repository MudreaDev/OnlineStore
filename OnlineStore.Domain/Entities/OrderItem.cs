using System;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        protected OrderItem() { }

        public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity, string? size = null, string? color = null)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Size = size;
            Color = color;
        }
    }
}
