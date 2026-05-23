using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Patterns.Visitor;
using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class FootwearProduct : Product, IPrototype<FootwearProduct>
    {
        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? AvailableSizes { get; set; } // e.g. "39,40,41,42"

        public FootwearProduct(string name, decimal price, string? size = null, string? material = null)
            : base(name, price)
        {
            Size = size;
            Material = material;
        }

        public FootwearProduct Clone()
        {
            return new FootwearProduct(Name + " (Copy)", Price, Size, Material)
            {
                Stock = this.Stock,
                AvailableSizes = this.AvailableSizes,
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails)
            };
        }

        public override string GetDescription()
        {
            return $"Footwear: {Name}, Material: {Material}, Available Sizes: {AvailableSizes}, Price: {Price:C}";
        }

        public override void Accept(IProductVisitor visitor)
        {
            visitor.VisitFootwearProduct(this);
        }
    }
}
