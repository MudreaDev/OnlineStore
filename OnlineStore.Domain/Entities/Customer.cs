using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class Customer : User
    {
        public string ShippingAddress { get; set; }
        public List<Order> OrderHistory { get; set; }

        public Customer(string username, string email, string shippingAddress) : base(username, email)
        {
            ShippingAddress = shippingAddress;
            OrderHistory = new List<Order>();
        }
    }
}
