using System.Collections.Generic;
using Xunit;

namespace Converto.UnitTests
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
            var result = dictionary.ToObject<Car>();

            // Assert
            Assert.Equal("Name of the car", result.Name);
            Assert.Null(result.Color);
            Assert.Null(result.Owner);
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
            var result = dictionary.ToObject<Car>();

            // Assert
            Assert.Equal("Name of the car", result.Name);
            Assert.Null(result.Color);
            Assert.Same(owner, result.Owner);
        }
    }
}
