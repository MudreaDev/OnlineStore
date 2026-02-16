using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class VehicleProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            // Default values as requested: Brand: "Generic", Model: "Standard", Year: 2024
            return new VehicleProduct(name, price, "Generic", "Standard", 2024);
        }
    }
}
