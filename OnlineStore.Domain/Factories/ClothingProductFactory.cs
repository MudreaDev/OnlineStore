using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class ClothingProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            return new ClothingProduct(name, price, "M", "Cotton"); // Mărime și material default
        }
    }
}
