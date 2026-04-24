using OnlineStore.Application.Patterns.TemplateMethod;
using OnlineStore.Domain.Entities;
using Xunit;
using System;
using System.Collections.Generic;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class TemplateMethodTests
    {
        [Fact]
        public void InvoiceGenerator_ShouldRunAllStepsWithoutError()
        {
            // Arrange
            var user = new User("Mihai", "test@email.com");
            var order = new Order(user, new List<OrderItem>(), 150, "Bucuresti", "0722");
            var generator = new InvoiceGenerator();

            // Act & Assert
            var exception = Record.Exception(() => generator.Generate(order));
            Assert.Null(exception);
        }

        [Fact]
        public void ShippingLabelGenerator_ShouldRunAllStepsWithoutError()
        {
            // Arrange
            var user = new User("Mihai", "test@email.com");
            var order = new Order(user, new List<OrderItem>(), 150, "Bucuresti", "0722");
            var generator = new ShippingLabelGenerator();

            // Act & Assert
            var exception = Record.Exception(() => generator.Generate(order));
            Assert.Null(exception);
        }
    }
}
