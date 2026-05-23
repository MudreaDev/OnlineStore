using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class FootwearProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            return new FootwearProduct(name, price, "40", "Leather"); 
        }
    }
}
