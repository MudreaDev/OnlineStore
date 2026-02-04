using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Factories
{
    // Factory Method: "Se va crea o clasă abstractă cu o metodă de fabrică ce returnează un obiect de tipul interfeței definite."
    public abstract class ProductFactory
    {
        public abstract Product CreateProduct(string name, decimal price);
    }
}
