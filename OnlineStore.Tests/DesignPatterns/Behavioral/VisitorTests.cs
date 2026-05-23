using OnlineStore.Application.Patterns.Visitor;
using OnlineStore.Domain.Entities;
using Xunit;
using System;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class VisitorTests
    {
        [Fact]
        public void ProductExportVisitor_ShouldExportAccessoryProductCorrectly()
        {
            // Arrange
            var product = new AccessoryProduct("Laptop", 1200, "Asus", "ROG", 24) { Stock = 10 };
            var visitor = new ProductExportVisitor();

            // Act
            product.Accept(visitor);
            var result = visitor.GetExportResult();

            // Assert
            Assert.Contains("--- ELECTRONIC PRODUCT EXPORT ---", result);
            Assert.Contains("Name: Laptop", result);
            Assert.Contains("Brand: Asus", result);
            Assert.Contains("Warranty: 24 months", result);
        }

        [Fact]
        public void ProductExportVisitor_ShouldExportClothingProductCorrectly()
        {
            // Arrange
            var product = new ClothingProduct("T-Shirt", 25, "L", "Cotton") { Stock = 50, AvailableColors = "Red, Blue" };
            var visitor = new ProductExportVisitor();

            // Act
            product.Accept(visitor);
            var result = visitor.GetExportResult();

            // Assert
            Assert.Contains("--- CLOTHING PRODUCT EXPORT ---", result);
            Assert.Contains("Name: T-Shirt", result);
            Assert.Contains("Size: L", result);
            Assert.Contains("Material: Cotton", result);
        }

        [Fact]
        public void ProductExportVisitor_ShouldExportFootwearProductCorrectly()
        {
            // Arrange
            var product = new FootwearProduct("Model S", 80000, "Tesla", "Long Range", 2023, "Electric") { Stock = 2 };
            var visitor = new ProductExportVisitor();

            // Act
            product.Accept(visitor);
            var result = visitor.GetExportResult();

            // Assert
            Assert.Contains("--- VEHICLE PRODUCT EXPORT ---", result);
            Assert.Contains("Name: Model S", result);
            Assert.Contains("Make: Tesla", result);
            Assert.Contains("Year: 2023", result);
        }
    }
}
