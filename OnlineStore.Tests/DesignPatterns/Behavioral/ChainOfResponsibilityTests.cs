using OnlineStore.Application.Patterns.ChainOfResponsibility;
using OnlineStore.Domain.Entities;
using Xunit;
using System.Collections.Generic;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class ChainOfResponsibilityTests
    {
        [Fact]
        public void OrderValidationChain_ShouldPass_WhenAllValid()
        {
            // Arrange
            var user = new User("test", "test@example.com");
            var items = new List<OrderItem> 
            { 
                new OrderItem(Guid.NewGuid(), "Product", 100, 2) 
            };

            var userHandler = new UserValidationHandler();
            var stockHandler = new StockValidationHandler();
            var priceHandler = new PricingValidationHandler();

            userHandler.SetNext(stockHandler).SetNext(priceHandler);

            // Act
            var result = userHandler.Handle(user, items);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void OrderValidationChain_ShouldFail_WhenUserIsNull()
        {
            // Arrange
            var items = new List<OrderItem> { new OrderItem(Guid.NewGuid(), "Product", 10, 1) };
            var userHandler = new UserValidationHandler();

            // Act
            var result = userHandler.Handle(null!, items);

            // Assert
            Assert.Equal("User cannot be null.", result);
        }

        [Fact]
        public void OrderValidationChain_ShouldFail_WhenQuantityIsInvalid()
        {
            // Arrange
            var user = new User("test", "test@example.com");
            var items = new List<OrderItem> 
            { 
                new OrderItem(Guid.NewGuid(), "Product", 100, 0) 
            };

            var userHandler = new UserValidationHandler();
            var stockHandler = new StockValidationHandler();
            userHandler.SetNext(stockHandler);

            // Act
            var result = userHandler.Handle(user, items);

            // Assert
            Assert.Contains("Invalid quantity", result);
        }
    }
}
