using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Decorator;

namespace OnlineStore.Tests
{
    public class DecoratorTests
    {
        [Fact]
        public void Decorator_ShouldChainMultipleBehaviors()
        {
            // Arrange
            string message = "Comandă finalizată!";
            INotification notification = new EmailNotification();
            notification = new SmsDecorator(notification);
            notification = new PushDecorator(notification);

            // Act
            string result = notification.Send(message);

            // Assert
            Assert.Contains("[Email]", result);
            Assert.Contains("[SMS]", result);
            Assert.Contains("[Push]", result);
        }

        [Fact]
        public void Decorator_SingleBehavior_ShouldWork()
        {
            // Arrange
            string message = "Test";
            INotification notification = new EmailNotification();
            notification = new SmsDecorator(notification);

            // Act
            string result = notification.Send(message);

            // Assert
            Assert.Equal("[Email] Trimis: Test + [SMS] Preluat: Test", result);
        }
    }
}
