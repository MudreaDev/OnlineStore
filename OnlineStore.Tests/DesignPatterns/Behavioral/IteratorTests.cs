using System.Collections.Generic;
using OnlineStore.Domain.DesignPatterns.Behavioral.Iterator;
using OnlineStore.Domain.Entities;
using Xunit;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class IteratorTests
    {
        [Fact]
        public void ProductTypeIterator_FiltersProperly()
        {
            // Arrange
            var products = new List<Product>
            {
                new ElectronicProduct("Phone", 1000, 24),
                new ClothingProduct("Shirt", 50, "M", "Cotton"),
                new ElectronicProduct("Laptop", 2000, 24)
            };

            var collection = new ProductCollection(products);
            
            // Act
            var iterator = collection.CreateIterator("Electronic");
            var filtered = new List<Product>();
            while (iterator.HasNext())
            {
                filtered.Add(iterator.Next());
            }

            // Assert
            Assert.Equal(2, filtered.Count);
            Assert.Contains(filtered, p => p.Name == "Phone");
            Assert.Contains(filtered, p => p.Name == "Laptop");
            Assert.DoesNotContain(filtered, p => p.Name == "Shirt");
        }
    }
}
