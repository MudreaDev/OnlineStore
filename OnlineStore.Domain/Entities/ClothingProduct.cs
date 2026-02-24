using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Entities
{
    //Aceste clase extind Product și implementează comportamente specifice fără a modifica clasa de bază,
    //respectând Open/Closed Principle.”  Demonstrează: OCP override LSP
    public class ClothingProduct : Product, IPrototype<ClothingProduct>
    {
        public string Size { get; set; } // Legacy or default
        public string Material { get; set; }
        public string? AvailableSizes { get; set; } // e.g. "S,M,L,XL"
        public string? AvailableColors { get; set; } // e.g. "Black,White,Navy"

        public ClothingProduct(string name, decimal price, string size, string material)
            : base(name, price)
        {
            Size = size;
            Material = material;
        }

        /// <summary>
        /// Implementare Pattern Prototype.
        /// Creează o copie a produsului de îmbrăcăminte cu un ID nou și nume marcat " (Copy)".
        /// </summary>
        public ClothingProduct Clone()
        {
            return new ClothingProduct(Name + " (Copy)", Price, Size, Material)
            {
                Stock = this.Stock,
                AvailableSizes = this.AvailableSizes,
                AvailableColors = this.AvailableColors
            };
        }

        public override string GetDescription()
        {
            var variants = "";
            if (!string.IsNullOrEmpty(AvailableSizes)) variants += $" Sizes: {AvailableSizes}";
            if (!string.IsNullOrEmpty(AvailableColors)) variants += $" Colors: {AvailableColors}";
            return $"Clothing: {Name}, Material: {Material}, Price: {Price:C}{variants}";
        }
    }
}
