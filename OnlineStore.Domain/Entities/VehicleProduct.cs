namespace OnlineStore.Domain.Entities
{
    public class VehicleProduct : Product
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

        public override string GetDescription()
        {
            return $"Vehicle: {Brand} {Model} ({Year}) - {Name}, Price: {Price:C}";
        }
    }
}
