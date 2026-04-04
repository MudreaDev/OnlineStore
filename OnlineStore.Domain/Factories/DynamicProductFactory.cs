using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class DynamicProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            return new DynamicProduct(name, price);
        }
    }
}
