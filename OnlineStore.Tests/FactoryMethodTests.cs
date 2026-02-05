using OnlineStore.Domain.Factories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Tests
{
    /// <summary>
    /// Teste pentru Factory Method Pattern
    /// </summary>
    public class FactoryMethodTests
    {
        [Fact]
        public void ElectronicFactory_CreatesElectronicProduct()
        {
            // Arrange
            ProductFactory factory = new ElectronicProductFactory();

            // Act
            Product product = factory.CreateProduct("Laptop", 1487m);

            // Assert
            Assert.IsType<ElectronicProduct>(product);
            Assert.Equal("Laptop", product.Name);
            Assert.Equal(1487m, product.Price);
        }

        [Fact]
        public void ClothingFactory_CreatesClothingProduct()
        {
            // Arrange
            ProductFactory factory = new ClothingProductFactory();

            // Act
            Product product = factory.CreateProduct("T-Shirt", 27m);

            // Assert
            Assert.IsType<ClothingProduct>(product);
            Assert.Equal("T-Shirt", product.Name);
            Assert.Equal(27m, product.Price);
        }

        [Fact]
        public void DifferentFactories_CreateDifferentProducts()
        {
            // Arrange
            ProductFactory electronicFactory = new ElectronicProductFactory();
            ProductFactory clothingFactory = new ClothingProductFactory();

            // Act
            Product electronic = electronicFactory.CreateProduct("Phone", 687m);
            Product clothing = clothingFactory.CreateProduct("Shirt", 36m);

            // Assert
            Assert.IsType<ElectronicProduct>(electronic);
            Assert.IsType<ClothingProduct>(clothing);
        }
    }
}