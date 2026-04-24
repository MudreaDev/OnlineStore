using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Patterns.Visitor;

namespace OnlineStore.Domain.Entities
{
    public class VehicleProduct : Product, IPrototype<VehicleProduct>
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? FuelType { get; set; }

        public VehicleProduct(string name, decimal price, string? make = null, string? model = null, int year = 0, string? fuelType = null)
            : base(name, price)
        {
            Make = make;
            Model = model;
            Year = year;
            FuelType = fuelType;
        }

        public VehicleProduct Clone()
        {
            return new VehicleProduct(Name + " (Copy)", Price, Make, Model, Year, FuelType)
            {
                Stock = this.Stock,
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails)
            };
        }

        public override string GetDescription()
        {
            return $"Vehicle: {Make} {Model} ({Year}) - {Name}, Price: {Price:C}";
        }

        public override void Accept(IProductVisitor visitor)
        {
            visitor.VisitVehicleProduct(this);
        }
    }
}
