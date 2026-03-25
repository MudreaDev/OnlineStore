using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Bridge;

namespace OnlineStore.Tests
{
    public class BridgeTests
    {
        [Fact]
        public void Bridge_HomeDeliveryViaCourier_ShouldWork()
        {
            // Arrange
            IShippingImplementation implementation = new CourierProvider();
            ShippingMethod method = new HomeDelivery(implementation);

            // Act
            string result = method.Deliver("PKG-100", "Str. Mihai Eminescu nr. 5");

            // Assert
            Assert.Contains("[Curier]", result);
            Assert.Contains("Adresă de domiciliu:", result);
        }

        [Fact]
        public void Bridge_PickupPointViaPostal_ShouldWork()
        {
            // Arrange
            IShippingImplementation implementation = new PostalProvider();
            ShippingMethod method = new PickupPointDelivery(implementation);

            // Act
            string result = method.Deliver("PKG-200", "Str. Victoriei nr. 10 (Easybox)");

            // Assert
            Assert.Contains("[Poștă]", result);
            Assert.Contains("Punct de ridicare:", result);
        }
    }
}
