using System;
using System.Linq;
using OnlineStore.Domain.DesignPatterns.Behavioral.Command;
using OnlineStore.Domain.DesignPatterns.Behavioral.Memento;
using OnlineStore.Domain.Entities;
using Xunit;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class CommandTests
    {
        [Fact]
        public void AddProductCommand_ExecutesSuccessfully()
        {
            // Arrange
            var cart = new ShoppingCart();
            var productId = Guid.NewGuid();
            var command = new AddProductCommand(cart, productId, "L", "Blue");

            // Act
            command.Execute();

            // Assert
            Assert.Single(cart.Items);
            Assert.Equal(productId, cart.Items.First().ProductId);
            Assert.Equal("L", cart.Items.First().Size);
        }

        [Fact]
        public void RemoveProductCommand_ExecutesSuccessfully()
        {
            // Arrange
            var cart = new ShoppingCart();
            var productId = Guid.NewGuid();
            cart.AddProduct(productId);
            var command = new RemoveProductCommand(cart, productId);

            // Act
            command.Execute();

            // Assert
            Assert.Empty(cart.Items);
        }

        [Fact]
        public void CartActionInvoker_ExecutesCommandAndSavesState()
        {
            // Arrange
            var cart = new ShoppingCart();
            var caretaker = new CartCaretaker();
            var invoker = new CartActionInvoker(caretaker);
            var productId = Guid.NewGuid();
            var command = new AddProductCommand(cart, productId);

            // Act
            invoker.ExecuteCommand(command, cart);

            // Assert
            Assert.Single(cart.Items); // Command executed
            Assert.Single(caretaker.UndoStack); // Memento saved before execution (empty cart)
            Assert.Empty(caretaker.UndoStack.Peek().ItemsSnapshot);
        }
    }
}
