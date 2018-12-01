using Xunit;

namespace Converto.UnitTests
{
    public class ConvertToTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }

        public class Plane
        {
            public string Name { get; set; }
            public string PrimaryColor { get; set; }
        }

        [Fact]
        public void ConvertTo_NullObject_Returns_NullObject()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.ConvertTo<Plane>();

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void TryConvertTo_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var canConvertTo = car.TryConvertTo<Plane>(out var result);

            // Assert
            Assert.False(canConvertTo);
            Assert.Null(result);
        }

        [Fact]
        public void ConvertTo_Object_Returns_NewObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.ConvertTo<Plane>();

            // Assert
            Assert.Equal(car.Name, result.Name);
            Assert.Null(result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }
        [Fact]
        public void TryConvertTo_Object_Returns_True()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canConvertTo = car.TryConvertTo<Plane>(out var result);

            // Assert
            Assert.True(canConvertTo);
            Assert.Equal(car.Name, result.Name);
            Assert.Null(result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }
    }
}
