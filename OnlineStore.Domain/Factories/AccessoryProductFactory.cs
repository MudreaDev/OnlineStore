using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class AccessoryProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            return new AccessoryProduct(name, price, "Generic Brand", "Mixed"); 
        }
    }
}
