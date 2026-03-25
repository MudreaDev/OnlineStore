using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Flyweight;

namespace OnlineStore.Tests
{
    public class FlyweightTests
    {
        [Fact]
        public void Flyweight_ShouldReuseInstances()
        {
            // Arrange
            var factory = new CharacterFactory();
            string document = "AAAAABBBCC";

            // Act
            foreach (char c in document)
            {
                var character = factory.GetCharacter(c);
                character.Display(10);
            }

            // Assert
            // Even though we "created" 10 characters, only 3 unique ones should exist in memory
            Assert.Equal(3, factory.GetTotalObjectsCreated());
        }

        [Fact]
        public void Flyweight_ShouldReturnSameInstanceForSameKey()
        {
            // Arrange
            var factory = new CharacterFactory();

            // Act
            var char1 = factory.GetCharacter('A');
            var char2 = factory.GetCharacter('A');

            // Assert
            Assert.Same(char1, char2);
        }
    }
}
