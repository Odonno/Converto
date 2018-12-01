using Xunit;

namespace Converto.SuccincT.UnitTests
{
    public class CopyTests
    {
        public class Car
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }
        
        [Fact]
        public void TryCopy_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.TryCopy();

            // Assert
            Assert.False(result.HasValue);
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
            var result = car.TryCopy();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(car.Name, result.Value.Name);
            Assert.Equal(car.Color, result.Value.Color);
            Assert.NotSame(car, result);
        }
    }
}
