namespace OnlineStore.Domain.Entities
{
    //Aceste clase extind Product și implementează comportamente specifice fără a modifica clasa de bază,
    //respectând Open/Closed Principle.”  Demonstrează: OCP override LSP
    public class ElectronicProduct : Product
    {
        public int WarrantyMonths { get; set; }

        public ElectronicProduct(string name, decimal price, int warrantyMonths) 
            : base(name, price)
        {
            WarrantyMonths = warrantyMonths;
        }

        public override string GetDescription()//polimorfism
        {
            return $"Electronic: {Name}, Price: {Price:C}, Warranty: {WarrantyMonths} months";
        }
    }
}
