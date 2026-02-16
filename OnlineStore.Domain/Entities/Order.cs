using System;
using System.Collections.Generic;
using OnlineStore.Domain.Common;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Domain.Entities
{
    public class Order : Entity
    {
        public User User { get; set; }
        public List<Product> Products { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        protected Order()
        {
            User = null!;
            Products = new List<Product>();
        }

        public Order(User user, List<Product> products, decimal total)
        {
            User = user;
            Products = products;
            Total = total;
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
        }
    }
}
