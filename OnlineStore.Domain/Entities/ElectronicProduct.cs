using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Patterns.Visitor;

namespace OnlineStore.Domain.Entities
{
    public class ElectronicProduct : Product, IPrototype<ElectronicProduct>
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int WarrantyMonths { get; set; }

        public ElectronicProduct(string name, decimal price, string? brand = null, string? model = null, int warrantyMonths = 0)
            : base(name, price)
        {
            Brand = brand;
            Model = model;
            WarrantyMonths = warrantyMonths;
        }

        public ElectronicProduct Clone()
        {
            return new ElectronicProduct(Name + " (Copy)", Price, Brand, Model, WarrantyMonths)
            {
                Stock = this.Stock,
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails)
            };
        }

        public override string GetDescription()
        {
            return $"Electronic: {Brand} {Model}, Price: {Price:C}, Warranty: {WarrantyMonths} months";
        }

        public override void Accept(IProductVisitor visitor)
        {
            visitor.VisitElectronicProduct(this);
        }
    }
}
