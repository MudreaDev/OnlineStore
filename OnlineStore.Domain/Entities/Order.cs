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

        protected Order()
        {
            User = null!;
            Items = new List<OrderItem>();
        }

        public Order(User user, List<OrderItem> items, decimal total)
        {
            User = user;
            Items = items;
            Total = total;
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
        }
    }
}
