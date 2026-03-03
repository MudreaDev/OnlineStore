using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Facade;

namespace OnlineStore.Tests
{
    public class FacadeTests
    {
        [Fact]
        public void OrderProcessingFacade_PlaceOrder_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            var facade = new OrderProcessingFacade();

            // Act
            bool result = facade.PlaceOrder("PROD_123", 2, "CUST_999", "client@test.com", 299.99m);

            // Assert
            Assert.True(result);
        }
    }
}
