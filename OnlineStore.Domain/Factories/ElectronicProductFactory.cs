using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    public class ElectronicProductFactory : ProductFactory
    {
        public override Product CreateProduct(string name, decimal price)
        {
            // Putem adăuga logică implicită sau specifică fabricii
            return new ElectronicProduct(name, price, 24); // Implicit 24 luni garanție
        }
    }
}
