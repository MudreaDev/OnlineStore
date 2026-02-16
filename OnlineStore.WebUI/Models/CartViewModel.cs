using OnlineStore.Domain.Entities;

namespace OnlineStore.WebUI.Models
{
    public class CartViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public decimal Total { get; set; }
    }
}
