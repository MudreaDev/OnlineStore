using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Composite;

namespace OnlineStore.Tests
{
    public class CompositeTests
    {
        [Fact]
        public void ProductItem_GetPrice_ShouldReturnItemPrice()
        {
            // Arrange
            ICatalogComponent product = new ProductItem("Laptop", 5000m);

            // Act
            decimal price = product.GetPrice();

            // Assert
            Assert.Equal(5000m, price);
        }

        [Fact]
        public void ProductCategory_GetPrice_ShouldReturnSumOfChildren()
        {
            // Arrange
            var electronics = new ProductCategory("Electronics");
            electronics.Add(new ProductItem("Laptop", 5000m));
            electronics.Add(new ProductItem("Mouse", 150m));

            var accessories = new ProductCategory("Accessories");
            accessories.Add(new ProductItem("Mousepad", 50m));

            // Nested Category
            electronics.Add(accessories);

            // Act
            decimal totalPrice = electronics.GetPrice();

            // Assert
            // 5000 + 150 + 50 = 5200
            Assert.Equal(5200m, totalPrice);
        }
    }
}
