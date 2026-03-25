using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Proxy;

namespace OnlineStore.Tests
{
    public class ProxyTests
    {
        [Fact]
        public void Proxy_AdminAccess_ShouldBeAllowed()
        {
            // Arrange
            IResource proxy = new ProtectionProxy();

            // Act
            string result = proxy.Access("Admin");

            // Assert
            Assert.Equal("Accesat resursă confidențială.", result);
        }

        [Fact]
        public void Proxy_UserAccess_ShouldBeDenied()
        {
            // Arrange
            IResource proxy = new ProtectionProxy();

            // Act
            string result = proxy.Access("User");

            // Assert
            Assert.Equal("Acces interzis: Doar administratorii pot accesa această resursă.", result);
        }
    }
}
