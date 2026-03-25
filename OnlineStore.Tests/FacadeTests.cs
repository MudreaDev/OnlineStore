using System;
using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Facade;
using OnlineStore.Domain.Entities;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.DesignPatterns.Structural.Adapter;
using OnlineStore.Domain.Factories;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Tests
{
    public class FacadeTests
    {
        [Fact]
        public void OrderProcessingFacade_Checkout_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            var productRepo = new InMemoryProductRepository();
            var orderRepo = new InMemoryOrderRepository();
            var userRepo = new InMemoryUserRepository();
            var paymentProcessor = new StripeAdapter(new StripeApi());

            var user = new Customer("test", "test@test.com", "addr");
            userRepo.Add(user);

            var factory = new ElectronicProductFactory();
            var product = factory.CreateProduct("Laptop Test", 1000m);
            product.Stock = 10;
            productRepo.Add(product);

            var cart = new ShoppingCart(user.Id);
            cart.AddProduct(product.Id);

            var facade = new OrderProcessingFacade(productRepo, productRepo, orderRepo, userRepo, paymentProcessor, new MockEmailService());

            // Act
            bool result = facade.Checkout(user, cart, out string message, out Order placedOrder);

            // Assert
            Assert.True(result);
            Assert.NotNull(placedOrder);
            Assert.Equal(9, product.Stock);
        }
    }

    public class MockEmailService : IEmailService
    {
        public System.Threading.Tasks.Task SendEmailAsync(string to, string subject, string body) => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task SendOrderConfirmationAsync(string userEmail, string orderId, decimal totalAmount) => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task SendPasswordResetCodeAsync(string email, string code) => System.Threading.Tasks.Task.CompletedTask;
    }
}
