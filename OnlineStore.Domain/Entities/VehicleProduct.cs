using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Entities
{
    public class VehicleProduct : Product, IPrototype<VehicleProduct>
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        public VehicleProduct(string name, decimal price, string brand, string model, int year)
            : base(name, price)
        {
            Brand = brand;
            Model = model;
            Year = year;
        }

        // Parameterless constructor for EF Core
        protected VehicleProduct()
        {
            Brand = null!;
            Model = null!;
        }

        /// <summary>
        /// Implementare Pattern Prototype.
        /// Creează o copie a produsului vehicul cu un ID nou și nume marcat " (Copy)".
        /// </summary>
        public VehicleProduct Clone()
        {
            return new VehicleProduct(Name + " (Copy)", Price, Brand, Model, Year)
            {
                Stock = this.Stock
            };
        }

        public override string GetDescription()
        {
            return $"Vehicle: {Brand} {Model} ({Year}) - {Name}, Price: {Price:C}";
        }
    }
}
