using Xunit;

namespace Converto.UnitTests
{
    public class CopyTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }

        [Fact]
        public void Copy_NullObject_Returns_NullObject()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.Copy();

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void TryCopy_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var canCopy = car.TryCopy(out var result);

            // Assert
            Assert.False(canCopy);
            Assert.Null(result);
        }

        [Fact]
        public void Copy_Object_Returns_NewObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.Copy();

            // Assert
            Assert.Equal(car.Name, result.Name);
            Assert.Equal(car.Color, result.Color);
            Assert.NotSame(car, result);
        }
        [Fact]
        public void TryCopy_Object_Returns_True()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canCopy = car.TryCopy(out var result);

            // Assert
            Assert.True(canCopy);
            Assert.Equal(car.Name, result.Name);
            Assert.Equal(car.Color, result.Color);
            Assert.NotSame(car, result);
        }
    }
}
