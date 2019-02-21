using Xunit;
using static Converto.Main;

namespace Converto.UnitTests
{
    public class IsEqualTests
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
        public void TwoNullObjects_Returns_True()
        {
            // Arrange
            Car car1 = null;
            Car car2 = null;

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OneNullObjectAndAnExistingOne_Returns_False()
        {
            // Arrange
            Car car1 = null;
            Car car2 = new Car
            {
                Name = "Name of the car 2",
                Color = "Red"
            };

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoCarsWithSameProperties_Returns_True()
        {
            // Arrange
            Car car1 = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };
            Car car2 = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoCarsWithDifferentProperties_Returns_False()
        {
            // Arrange
            Car car1 = new Car
            {
                Name = "Name of the car 1",
                Color = "Red"
            };
            Car car2 = new Car
            {
                Name = "Name of the car 2",
                Color = "Red"
            };

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoCarsWithSameNestedProperties_Returns_True()
        {
            // Arrange
            Car car1 = new Car
            {
                Name = "Name of the car",
                Color = "Red",
                Owner = new Owner
                {
                    FirstName = "Freddy",
                    LastName = "Mercury"
                }
            };
            Car car2 = new Car
            {
                Name = "Name of the car",
                Color = "Red",
                Owner = new Owner
                {
                    FirstName = "Freddy",
                    LastName = "Mercury"
                }
            };

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoCarsWithDifferentNestedProperties_Returns_False()
        {
            // Arrange
            Car car1 = new Car
            {
                Name = "Name of the car 1",
                Color = "Red",
                Owner = new Owner
                {
                    FirstName = "Freddy",
                    LastName = "Mercury"
                }
            };
            Car car2 = new Car
            {
                Name = "Name of the car 2",
                Color = "Red",
                Owner = new Owner
                {
                    FirstName = "Brian",
                    LastName = "May"
                }
            };

            // Act
            bool result = IsEqual(car1, car2);

            // Assert
            Assert.False(result);
        }
    }
}
