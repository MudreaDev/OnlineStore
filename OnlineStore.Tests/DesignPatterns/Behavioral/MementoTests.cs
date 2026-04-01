using System;
using System.Linq;
using OnlineStore.Domain.DesignPatterns.Behavioral.Command;
using OnlineStore.Domain.DesignPatterns.Behavioral.Memento;
using OnlineStore.Domain.Entities;
using Xunit;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class MementoTests
    {
        [Fact]
        public void ShoppingCart_SavesAndRestoresState()
        {
            // Arrange
            var cart = new ShoppingCart();
            var productId = Guid.NewGuid();
            cart.AddProduct(productId);

            // Save state (Memento 1: Product 1)
            var memento = cart.SaveState();

            // Change state
            cart.AddProduct(Guid.NewGuid()); // Add another
            cart.RemoveProduct(productId);   // Remove first

            // Act
            cart.RestoreState(memento);

            // Assert
            Assert.Single(cart.Items);
            Assert.Equal(productId, cart.Items.First().ProductId);
        }

        [Fact]
        public void CartActionInvoker_UndoRedo_WorksCorrectly()
        {
            // Arrange
            var cart = new ShoppingCart();
            var caretaker = new CartCaretaker();
            var invoker = new CartActionInvoker(caretaker);
            
            var p1Id = Guid.NewGuid();
            var p2Id = Guid.NewGuid();

            // Action 1: Add P1
            invoker.ExecuteCommand(new AddProductCommand(cart, p1Id), cart);
            
            // Action 2: Add P2
            invoker.ExecuteCommand(new AddProductCommand(cart, p2Id), cart);
            
            Assert.Equal(2, cart.Items.Count);

            // Act - Undo
            invoker.Undo(cart);
            
            // Assert - State restored to just P1
            Assert.Single(cart.Items);
            Assert.Equal(p1Id, cart.Items.First().ProductId);

            // Act - Undo again
            invoker.Undo(cart);

            // Assert - State restored to Empty
            Assert.Empty(cart.Items);

            // Act - Redo
            invoker.Redo(cart);

            // Assert - State restored to P1
            Assert.Single(cart.Items);

            // Act - Redo again
            invoker.Redo(cart);

            // Assert - State restored to P1 and P2
            Assert.Equal(2, cart.Items.Count);
        }
    }
}
