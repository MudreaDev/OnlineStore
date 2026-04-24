using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Patterns.Visitor;

namespace OnlineStore.Domain.Entities
{
    public class DynamicProduct : Product, IPrototype<DynamicProduct>
    {
        public Dictionary<string, string> CustomAttributes { get; set; } = new Dictionary<string, string>();

        public DynamicProduct(string name, decimal price) : base(name, price)
        {
        }

        protected DynamicProduct() { }

        public DynamicProduct Clone()
        {
            return new DynamicProduct(Name + " (Copy)", Price)
            {
                Stock = this.Stock,
                SubCategoryId = this.SubCategoryId,
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails),
                CustomAttributes = new Dictionary<string, string>(this.CustomAttributes)
            };
        }

        public override string GetDescription()
        {
            return $"Dynamic: {Name}, Price: {Price:C}";
        }

        public override void Accept(IProductVisitor visitor)
        {
            visitor.VisitDynamicProduct(this);
        }
    }
}
