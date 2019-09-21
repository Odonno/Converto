using System.Collections.Generic;
using Xunit;

namespace Converto.UnitTests
{
    public class ToDictionaryTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
            public int? ProductionYear { get; set; }
            public Owner Owner { get; set; }
        }

        public class Owner
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Fact]
        public void AnObject_Returns_ADictionary()
        {
            // Arrange
            var owner = new Owner
            {
                FirstName = "Freddy",
                LastName = "Mercury"
            };

            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red",
                Owner = owner
            };

            // Act
            var result = car.ToDictionary();

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Name of the car", result["Name"]);
            Assert.Equal("Red", result["Color"]);
            Assert.Null(result["ProductionYear"]);
            Assert.Same(owner, result["Owner"]);
        }

        [Fact]
        public void AnObject_Returns_ADictionaryInRecursiveMode()
        {
            // Arrange
            var owner = new Owner
            {
                FirstName = "Freddy",
                LastName = "Mercury"
            };

            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red",
                ProductionYear = 1999,
                Owner = owner
            };

            // Act
            var result = car.ToDictionary(true);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Name of the car", result["Name"]);
            Assert.Equal("Red", result["Color"]);
            Assert.Equal(1999, result["ProductionYear"]);

            Assert.NotSame(owner, result["Owner"]);
            Assert.IsType<Dictionary<string, object>>(result["Owner"]);

            var ownerDictionary = result["Owner"] as Dictionary<string, object>;
            Assert.Equal("Freddy", ownerDictionary["FirstName"]);
            Assert.Equal("Mercury", ownerDictionary["LastName"]);
        }
    }
}
