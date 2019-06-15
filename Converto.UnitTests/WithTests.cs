using Xunit;

namespace Converto.UnitTests
{
    public class WithTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }

        public class EmptyProps { }

        [Fact]
        public void With_NullObject_Returns_NullObject()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.With(new { Color = "Blue" });

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void TryWith_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var canWith = car.TryWith(new { Color = "Blue" }, out var result);

            // Assert
            Assert.False(canWith);
            Assert.Null(result);
        }

        [Fact]
        public void With_NullPropsToUpdate_Returns_NullObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.With(null as EmptyProps);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void TryWith_NullPropsToUpdate_Returns_False()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canWith = car.TryWith(null as EmptyProps, out var result);

            // Assert
            Assert.False(canWith);
            Assert.Null(result);
        }

        [Fact]
        public void With_ChangingColor_Returns_NewObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.With(new { Color = "Blue" });

            // Assert
            Assert.Equal(car.Name, result.Name);
            Assert.Equal("Blue", result.Color);
            Assert.NotSame(car, result);
        }
        [Fact]
        public void TryWith_ChangingColor_Returns_True()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canWith = car.TryWith(new { Color = "Blue" }, out var result);

            // Assert
            Assert.True(canWith);
            Assert.Equal(car.Name, result.Name);
            Assert.Equal("Blue", result.Color);
            Assert.NotSame(car, result);
        }
    }
}
