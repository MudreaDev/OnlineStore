using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Patterns.Visitor;
using System.Text;

namespace OnlineStore.Application.Patterns.Visitor
{
    /// <summary>
    /// Pattern Visitor — permite adăugarea de noi operații (ex: export inventar) pe structuri de obiecte 
    /// existente (produse) fără a modifica clasele acestora.
    /// Respectă Open/Closed Principle din SOLID.
    /// </summary>
    public class ProductExportVisitor : IProductVisitor
    {
        private readonly StringBuilder _exportData = new StringBuilder();

        public string GetExportResult() => _exportData.ToString();

        public void VisitFootwearProduct(FootwearProduct product)
        {
            _exportData.AppendLine($"--- FOOTWEAR PRODUCT EXPORT ---");
            _exportData.AppendLine($"Name: {product.Name}");
            _exportData.AppendLine($"Price: {product.Price:C}");
            _exportData.AppendLine($"Size: {product.Size}");
            _exportData.AppendLine($"Material: {product.Material}");
            _exportData.AppendLine($"Available Sizes: {product.AvailableSizes}");
            _exportData.AppendLine($"Stock Status: {(product.Stock > 0 ? "In Stock" : "Out of Stock")} ({product.Stock} units)");
            _exportData.AppendLine("----------------------------------");
        }

        public void VisitClothingProduct(ClothingProduct product)
        {
            _exportData.AppendLine($"--- CLOTHING PRODUCT EXPORT ---");
            _exportData.AppendLine($"Name: {product.Name}");
            _exportData.AppendLine($"Price: {product.Price:C}");
            _exportData.AppendLine($"Size: {product.Size}");
            _exportData.AppendLine($"Material: {product.Material}");
            _exportData.AppendLine($"Available Colors: {product.AvailableColors}");
            _exportData.AppendLine($"Stock Status: {(product.Stock > 0 ? "In Stock" : "Out of Stock")} ({product.Stock} units)");
            _exportData.AppendLine("----------------------------------");
        }

        public void VisitAccessoryProduct(AccessoryProduct product)
        {
            _exportData.AppendLine($"--- ACCESSORY PRODUCT EXPORT ---");
            _exportData.AppendLine($"Name: {product.Name}");
            _exportData.AppendLine($"Price: {product.Price:C}");
            _exportData.AppendLine($"Brand: {product.Brand}");
            _exportData.AppendLine($"Material: {product.Material}");
            _exportData.AppendLine($"Stock Status: {(product.Stock > 0 ? "In Stock" : "Out of Stock")} ({product.Stock} units)");
            _exportData.AppendLine("----------------------------------");
        }

        public void VisitDynamicProduct(DynamicProduct product)
        {
            _exportData.AppendLine($"--- DYNAMIC PRODUCT EXPORT ---");
            _exportData.AppendLine($"Name: {product.Name}");
            _exportData.AppendLine($"Price: {product.Price:C}");
            foreach (var attr in product.CustomAttributes)
            {
                _exportData.AppendLine($"{attr.Key}: {attr.Value}");
            }
            _exportData.AppendLine($"Stock Status: {(product.Stock > 0 ? "In Stock" : "Out of Stock")} ({product.Stock} units)");
            _exportData.AppendLine("----------------------------------");
        }
    }
}
