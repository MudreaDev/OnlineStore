using OnlineStore.Domain.Common;
//Product este o clasă abstractă ce definește comportamentul comun tuturor produselor. \
//Folosesc încapsulare pentru validarea proprietăților și o metodă abstractă pentru polimorfism.”
//Demonstrează: încapsulare  moștenire  polimorfism  SRP
namespace OnlineStore.Domain.Entities
{
    public abstract class Product : Entity //adaug functional, extind clasa OCP
    {
        private string _name = null!;
        private decimal _price;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Product name cannot be empty.");
                _name = value;
            }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative.");
                _price = value;
            }
        }
        public int Stock { get; set; }

        protected Product()
        {
            _name = null!;
        }

        protected Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public abstract string GetDescription();
    }
}
