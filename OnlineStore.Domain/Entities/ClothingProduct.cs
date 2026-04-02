using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Entities
{
    //Aceste clase extind Product și implementează comportamente specifice fără a modifica clasa de bază,
    //respectând Open/Closed Principle.”  Demonstrează: OCP override LSP
    public class ClothingProduct : Product, IPrototype<ClothingProduct>
    {
        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? AvailableSizes { get; set; } // e.g. "S,M,L,XL"

        public ClothingProduct(string name, decimal price, string? size = null, string? material = null)
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
                AvailableColors = this.AvailableColors,
                SubscriberEmails = new List<string>(this.SubscriberEmails)
            };
        }

        public override string GetDescription()
        {
            return $"Modern {Material} apparel designed for comfort and style.";
        }
    }
}
