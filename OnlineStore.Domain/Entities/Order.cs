using System;
using System.Collections.Generic;
using OnlineStore.Domain.Common;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Domain.Entities
{
    public class Order : Entity
    {
        public User User { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        protected Order()
        {
            User = null!;
            Items = new List<OrderItem>();
        }

        public Order(User user, List<OrderItem> items, decimal total, string shippingAddress, string phoneNumber)
        {
            User = user;
            Items = items;
            Total = total;
            ShippingAddress = shippingAddress;
            PhoneNumber = phoneNumber;
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
        }
    }
}
