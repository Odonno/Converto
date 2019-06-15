using Xunit;

namespace Converto.SuccincT.UnitTests
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
        public void TryConvertWith_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.TryConvertWith<Plane, object>(new { X = 2 });

            // Assert
            Assert.False(result.HasValue);
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
            var result = car.TryConvertWith<Plane, object>(new { X = 2 });

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(car.Name, result.Value.Name);
            Assert.Null(result.Value.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result.Value);
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
            var result = car.TryConvertWith<Plane, object>(new { PrimaryColor = car.Color });

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(car.Name, result.Value.Name);
            Assert.Equal(car.Color, result.Value.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result.Value);
        }
    }
}
