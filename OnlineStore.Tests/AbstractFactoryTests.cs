using OnlineStore.Domain.Interfaces;
using OnlineStore.Application.Services;

namespace OnlineStore.Tests
{
    /// <summary>
    /// Teste pentru Abstract Factory Pattern
    /// </summary>
    public class AbstractFactoryTests
    {
        [Fact]
        public void LocalFactory_CreatesLocalServices()
        {
            // Arrange
            IStoreServicesFactory factory = new LocalStoreServicesFactory();

            // Act
            IPaymentProcessor payment = factory.CreatePaymentProcessor();
            IShippingProvider shipping = factory.CreateShippingProvider();

            // Assert
            Assert.IsType<LocalPaymentProcessor>(payment);
            Assert.IsType<LocalShippingProvider>(shipping);
        }

        [Fact]
        public void GlobalFactory_CreatesGlobalServices()
        {
            // Arrange
            IStoreServicesFactory factory = new GlobalStoreServicesFactory();

            // Act
            IPaymentProcessor payment = factory.CreatePaymentProcessor();
            IShippingProvider shipping = factory.CreateShippingProvider();

            // Assert
            Assert.IsType<GlobalPaymentProcessor>(payment);
            Assert.IsType<GlobalShippingProvider>(shipping);
        }

        [Fact]
        public void DifferentFactories_CreateConsistentFamilies()
        {
            // Arrange
            IStoreServicesFactory localFactory = new LocalStoreServicesFactory();
            IStoreServicesFactory globalFactory = new GlobalStoreServicesFactory();

            // Act
            var localPayment = localFactory.CreatePaymentProcessor();
            var globalPayment = globalFactory.CreatePaymentProcessor();

            // Assert
            Assert.NotEqual(localPayment.GetType(), globalPayment.GetType());
        }
    }
}