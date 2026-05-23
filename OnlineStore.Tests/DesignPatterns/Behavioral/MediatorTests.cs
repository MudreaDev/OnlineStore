using Moq;
using OnlineStore.Application.Patterns.Mediator;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using Xunit;
using System;
using System.Collections.Generic;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class MediatorTests
    {
        [Fact]
        public async Task Checkout_ShouldCoordinateServicesAndClearCart()
        {
            // Arrange
            var mockDiscount = new Mock<IDiscountStrategy>();
            var mockOrderService = new Mock<OrderService>(mockDiscount.Object);
            var mockEmailService = new Mock<IEmailService>();
            var mockProductRepo = new Mock<IReadableRepository<Product>>();
            var mockStockService = new Mock<IStockService>();
            var mockPaymentService = new Mock<IPaymentService>();
            var mockValidationService = new Mock<OrderValidationService>();
            var mockOrderRepo = new Mock<IWriteableRepository<Order>>();

            // Setup mocks
            var user = new User("test", "test@example.com");
            var orderId = Guid.NewGuid();
            var order = new Order(user, new List<OrderItem>(), 100, "Addr", "123");

            mockStockService.Setup(s => s.IsInStock(It.IsAny<Guid>(), It.IsAny<int>())).Returns(true);
            mockProductRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(new AccessoryProduct("Test", 100));

            mockOrderService.Setup(s => s.PlaceOrder(
                It.IsAny<User>(), 
                It.IsAny<List<OrderItem>>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .Returns(order);

            mockEmailService.Setup(s => s.SendOrderConfirmationAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<decimal>()))
                .Returns(Task.CompletedTask);

            var cart = new ShoppingCart();
            cart.AddProduct(Guid.NewGuid()); // Fixed method name

            var mediator = new CheckoutMediator(
                mockOrderService.Object, 
                mockValidationService.Object,
                mockEmailService.Object, 
                mockStockService.Object, 
                mockPaymentService.Object, 
                mockProductRepo.Object,
                mockOrderRepo.Object);

            // Act
            await mediator.CheckoutAsync(user, cart, "Stripe", "Local", "Home", "Courier", "Test Address", "0740000000");

            // Assert
            mockOrderService.Verify(s => s.PlaceOrder(user, It.IsAny<List<OrderItem>>(), "Test Address", "0740000000"), Times.Once);
            mockPaymentService.Verify(s => s.Process(It.IsAny<decimal>()), Times.Once);
            mockStockService.Verify(s => s.UpdateStock(It.IsAny<Guid>(), It.IsAny<int>()), Times.AtLeastOnce);
            Assert.Empty(cart.Items); // Cart should be cleared
        }
    }
}
