using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Patterns.Visitor
{
    public interface IProductVisitor
    {
        void VisitClothingProduct(ClothingProduct product);
        void VisitFootwearProduct(FootwearProduct product);
        void VisitAccessoryProduct(AccessoryProduct product);
        void VisitDynamicProduct(DynamicProduct product);
    }
}
