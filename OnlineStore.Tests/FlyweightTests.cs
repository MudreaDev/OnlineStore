using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Flyweight;

namespace OnlineStore.Tests
{
    public class FlyweightTests
    {
        [Fact]
        public void ProductTypeFlyweight_ShouldReuseInstances()
        {
            // Arrange
            ProductTypeFlyweightFactory.Reset();

            // Act — cerem flyweight-uri pentru 10 produse din 3 tipuri
            ProductTypeFlyweightFactory.GetFlyweight("Electronic");
            ProductTypeFlyweightFactory.GetFlyweight("Electronic");
            ProductTypeFlyweightFactory.GetFlyweight("Clothing");
            ProductTypeFlyweightFactory.GetFlyweight("Clothing");
            ProductTypeFlyweightFactory.GetFlyweight("Clothing");
            ProductTypeFlyweightFactory.GetFlyweight("Vehicle");
            ProductTypeFlyweightFactory.GetFlyweight("Electronic");
            ProductTypeFlyweightFactory.GetFlyweight("Vehicle");
            ProductTypeFlyweightFactory.GetFlyweight("Electronic");
            ProductTypeFlyweightFactory.GetFlyweight("Clothing");

            // Assert — 10 request-uri, dar doar 3 instanțe create
            Assert.Equal(3, ProductTypeFlyweightFactory.CacheSize);
            Assert.Equal(10, ProductTypeFlyweightFactory.TotalRequests);
            Assert.Equal(3, ProductTypeFlyweightFactory.TotalCreated);
        }

        [Fact]
        public void ProductTypeFlyweight_ShouldReturnSameInstanceForSameType()
        {
            // Arrange
            ProductTypeFlyweightFactory.Reset();

            // Act
            var fw1 = ProductTypeFlyweightFactory.GetFlyweight("Electronic");
            var fw2 = ProductTypeFlyweightFactory.GetFlyweight("Electronic");

            // Assert — exact aceeași instanță
            Assert.Same(fw1, fw2);
        }

        [Fact]
        public void ProductTypeFlyweight_RenderBadge_ContainsExtrinsicState()
        {
            // Arrange
            ProductTypeFlyweightFactory.Reset();
            var fw = ProductTypeFlyweightFactory.GetFlyweight("Electronic");

            // Act
            var badge = fw.RenderBadge("Samsung Galaxy S25");

            // Assert — badge conține starea extrinsecă (productName)
            Assert.Contains("Samsung Galaxy S25", badge);
            Assert.Contains("Electronic", badge);
        }
    }
}
