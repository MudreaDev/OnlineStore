using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Patterns.Visitor;
using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class AccessoryProduct : Product, IPrototype<AccessoryProduct>
    {
        public string? Material { get; set; }
        public string? Brand { get; set; }

        public AccessoryProduct(string name, decimal price, string? brand = null, string? material = null)
            : base(name, price)
        {
            Brand = brand;
            Material = material;
        }

        public AccessoryProduct Clone()
        {
            return new AccessoryProduct(Name + " (Copy)", Price, Brand, Material)
            {
                Stock = this.Stock,
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails)
            };
        }

        public override string GetDescription()
        {
            return $"Accessory: {Name}, Brand: {Brand}, Material: {Material}, Price: {Price:C}";
        }

        public override void Accept(IProductVisitor visitor)
        {
            visitor.VisitAccessoryProduct(this);
        }
    }
}
