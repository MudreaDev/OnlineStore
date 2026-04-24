using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Patterns.Visitor
{
    public interface IProductVisitor
    {
        void VisitElectronicProduct(ElectronicProduct product);
        void VisitClothingProduct(ClothingProduct product);
        void VisitVehicleProduct(VehicleProduct product);
        void VisitDynamicProduct(DynamicProduct product);
    }
}
