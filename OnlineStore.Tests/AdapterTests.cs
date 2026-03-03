using System;
using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Adapter;

namespace OnlineStore.Tests
{
    public class AdapterTests
    {
        [Fact]
        public void PayPalAdapter_ProcessPayment_ShouldReturnTrue()
        {
            // Arrange
            var payPalApi = new PayPalApi();
            IExternalPaymentProcessor paymentProcessor = new PayPalAdapter(payPalApi);
            decimal amount = 150m;
            string currency = "USD";
            string orderId = "ORD12345";

            // Act
            bool result = paymentProcessor.ProcessPayment(amount, currency, orderId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void StripeAdapter_ProcessPayment_ShouldReturnTrue()
        {
            // Arrange
            var stripeApi = new StripeApi();
            IExternalPaymentProcessor paymentProcessor = new StripeAdapter(stripeApi);
            decimal amount = 200m;
            string currency = "EUR";
            string orderId = "ORD98765";

            // Act
            bool result = paymentProcessor.ProcessPayment(amount, currency, orderId);

            // Assert
            Assert.True(result);
        }
    }
}
