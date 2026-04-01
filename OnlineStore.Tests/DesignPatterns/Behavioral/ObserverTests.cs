using Moq;
using OnlineStore.Application.Services;
using OnlineStore.Domain.DesignPatterns.Behavioral.Observer;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Interfaces;
using Xunit;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class ObserverTests
    {
        [Fact]
        public void OrderNotificationService_NotifiesAllObservers()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var notificationService = new OrderNotificationService(mockEmailService.Object);

            var user = new Customer("test", "test@test.com", "Test Addr");
            var order = new Order(user, new System.Collections.Generic.List<OrderItem>(), 100m) { Status = OrderStatus.Shipped };

            // NotificationService already attaches Email, Sms, Dashboard in constructor,
            // but let's test a custom mock observer as well to verify Notification logic.
            var mockObserver = new Mock<IOrderObserver>();
            notificationService.Attach(mockObserver.Object);

            // Act
            notificationService.Notify(order);

            // Assert
            mockObserver.Verify(o => o.Update(order), Times.Once);
            // EmailObserver is also attached by default, so it should have called EmailService
            mockEmailService.Verify(e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains(order.Id.ToString())),
                It.Is<string>(s => s.Contains("Shipped"))
            ), Times.Once);
        }
    }
}
