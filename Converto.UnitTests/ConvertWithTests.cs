using Xunit;

namespace Converto.UnitTests
{
    public class ConvertWithTests
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
        public void ConvertWith_NullObject_Returns_NullObject()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.ConvertWith<Plane, object>(new { X = 2 });

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void TryConvertWith_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var canConvertWith = car.TryConvertWith<Plane, object>(new { X = 2 }, out var result);

            // Assert
            Assert.False(canConvertWith);
            Assert.Null(result);
        }

        [Fact]
        public void ConvertWith_Object_AndMutateInexistingProperties_Returns_NewObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.ConvertWith<Plane, object>(new { X = 2 });

            // Assert
            Assert.Equal(car.Name, result.Name);
            Assert.Null(result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }
        [Fact]
        public void TryConvertWith_Object_AndMutateInexistingProperties_Returns_True()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canConvertWith = car.TryConvertWith<Plane, object>(new { X = 2 }, out var result);

            // Assert
            Assert.True(canConvertWith);
            Assert.Equal(car.Name, result.Name);
            Assert.Null(result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }

        [Fact]
        public void ConvertWith_Object_AndMutateExistingProperties_Returns_NewObject()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var result = car.ConvertWith<Plane, object>(new { PrimaryColor = car.Color });

            // Assert
            Assert.Equal(car.Name, result.Name);
            Assert.Equal(car.Color, result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }
        [Fact]
        public void TryConvertWith_Object_AndMutateExistingProperties_Returns_True()
        {
            // Arrange
            var car = new Car
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            var canConvertWith = car.TryConvertWith<Plane, object>(new { PrimaryColor = car.Color }, out var result);

            // Assert
            Assert.True(canConvertWith);
            Assert.Equal(car.Name, result.Name);
            Assert.Equal(car.Color, result.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result);
        }
    }
}
