using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Patterns.State;
using Xunit;
using System;
using System.Collections.Generic;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class StateTests
    {
        [Fact]
        public void Order_ShouldTransitionFromPendingToPaid_WhenPaid()
        {
            // Arrange
            var order = new Order(new User("test", "test@email.com"), new List<OrderItem>(), 100, "Address", "1234");
            
            // Act
            order.Pay();

            // Assert
            Assert.Equal("Paid", order.GetStateName());
            Assert.Equal(OrderStatus.Paid, order.Status);
        }

        [Fact]
        public void Order_ShouldTransitionFromPaidToShipped_WhenShipped()
        {
            // Arrange
            var order = new Order(new User("test", "test@email.com"), new List<OrderItem>(), 100, "Address", "1234");
            order.Pay();
            
            // Act
            order.Ship();

            // Assert
            Assert.Equal("Shipped", order.GetStateName());
            Assert.Equal(OrderStatus.Shipped, order.Status);
        }

        [Fact]
        public void Order_ShouldThrowException_WhenShippingPendingOrder()
        {
            // Arrange
            var order = new Order(new User("test", "test@email.com"), new List<OrderItem>(), 100, "Address", "1234");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.Ship());
        }

        [Fact]
        public void Order_ShouldTransitionToCancelled_WhenCancelledFromPending()
        {
            // Arrange
            var order = new Order(new User("test", "test@email.com"), new List<OrderItem>(), 100, "Address", "1234");
            
            // Act
            order.Cancel();

            // Assert
            Assert.Equal("Cancelled", order.GetStateName());
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
    }
}
