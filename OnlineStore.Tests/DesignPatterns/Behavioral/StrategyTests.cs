using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Strategies;
using Xunit;

namespace OnlineStore.Tests.DesignPatterns.Behavioral
{
    public class StrategyTests
    {
        [Fact]
        public void SortingStrategies_WorkCorrectly()
        {
            // Arrange
            var products = new List<Product>
            {
                new ElectronicProduct("Z", 100, 12) { Stock = 10 },
                new ElectronicProduct("A", 300, 24) { Stock = 50 },
                new ElectronicProduct("M", 50, 6) { Stock = 0 }
            };

            // Act & Assert
            var priceAsc = new PriceAscendingStrategy().Sort(products).ToList();
            Assert.Equal("M", priceAsc[0].Name);
            Assert.Equal("A", priceAsc[2].Name);

            var priceDesc = new PriceDescendingStrategy().Sort(products).ToList();
            Assert.Equal("A", priceDesc[0].Name);
            Assert.Equal("M", priceDesc[2].Name);

            var nameAsc = new NameSortingStrategy().Sort(products).ToList();
            Assert.Equal("A", nameAsc[0].Name);
            Assert.Equal("Z", nameAsc[2].Name);

            var stockDesc = new StockSortingStrategy().Sort(products).ToList();
            Assert.Equal("A", stockDesc[0].Name);
            Assert.Equal("M", stockDesc[2].Name);
        }
    }
}
