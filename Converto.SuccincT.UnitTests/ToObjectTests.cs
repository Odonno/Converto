using System.Collections.Generic;
using Xunit;

namespace Converto.SuccincT.UnitTests
{
    public class ToObjectTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
            public Owner Owner { get; set; }
        }

        public class Owner
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Fact]
        public void ADictionary_Returns_ANewObject()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                { "Name", "Name of the car" }
            };

            // Act
            var result = dictionary.TryToObject<Car>();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("Name of the car", result.Value.Name);
            Assert.Null(result.Value.Color);
            Assert.Null(result.Value.Owner);
        }

        [Fact]
        public void ADictionary_Returns_ANewComplexObject()
        {
            // Arrange
            var owner = new Owner
            {
                FirstName = "Freddy",
                LastName = "Mercury"
            };

            var dictionary = new Dictionary<string, object>
            {
                { "Name", "Name of the car" },
                { "Owner", owner }
            };

            // Act
            var result = dictionary.TryToObject<Car>();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("Name of the car", result.Value.Name);
            Assert.Null(result.Value.Color);
            Assert.Same(owner, result.Value.Owner);
        }
    }
}
