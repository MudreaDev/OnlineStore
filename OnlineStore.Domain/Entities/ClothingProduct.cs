namespace OnlineStore.Domain.Entities
{
    //Aceste clase extind Product și implementează comportamente specifice fără a modifica clasa de bază,
    //respectând Open/Closed Principle.”  Demonstrează: OCP override LSP
    public class ClothingProduct : Product
    {
        public string Size { get; set; }
        public string Material { get; set; }

        public ClothingProduct(string name, decimal price, string size, string material) 
            : base(name, price)
        {
            Size = size;
            Material = material;
        }

        public override string GetDescription()
        {
            return $"Clothing: {Name}, Size: {Size}, Material: {Material}, Price: {Price:C}";
        }
    }
}
