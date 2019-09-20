using Xunit;

namespace Converto.UnitTests
{
    public class ToDictionaryTests
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
            Assert.Equal(3, result.Count);
            Assert.Equal("Name of the car", result["Name"]);
            Assert.Equal("Red", result["Color"]);
            Assert.Same(owner, result["Owner"]);
        }
    }
}
