using Xunit;

namespace Converto.SuccincT.UnitTests
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
        public void TryConvertTo_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.TryConvertTo<Plane>();

            // Assert
            Assert.False(result.HasValue);
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
            var result = car.TryConvertTo<Plane>();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(car.Name, result.Value.Name);
            Assert.Null(result.Value.PrimaryColor);
            Assert.NotSame(car, result);
            Assert.IsType<Plane>(result.Value);
        }
    }
}
