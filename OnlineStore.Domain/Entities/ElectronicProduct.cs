using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Entities
{
    //Aceste clase extind Product și implementează comportamente specifice fără a modifica clasa de bază,
    //respectând Open/Closed Principle.”  Demonstrează: OCP override LSP
    public class ElectronicProduct : Product, IPrototype<ElectronicProduct>
    {
        public int WarrantyMonths { get; set; }

        public ElectronicProduct(string name, decimal price, int warrantyMonths)
            : base(name, price)
        {
            WarrantyMonths = warrantyMonths;
        }

        /// <summary>
        /// Implementare Pattern Prototype.
        /// Creează o copie a produsului electronic cu un ID nou și nume marcat " (Copy)".
        /// </summary>
        public ElectronicProduct Clone()
        {
            return new ElectronicProduct(Name + " (Copy)", Price, WarrantyMonths);
        }

        public override string GetDescription()//polimorfism
        {
            return $"Electronic: {Name}, Price: {Price:C}, Warranty: {WarrantyMonths} months";
        }
    }
}
