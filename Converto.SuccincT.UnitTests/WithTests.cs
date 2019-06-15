using Xunit;

namespace Converto.SuccincT.UnitTests
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
        public void TryWith_NullObject_Returns_False()
        {
            // Arrange
            Car car = null;

            // Act
            var result = car.TryWith(new { Color = "Blue" });

            // Assert
            Assert.False(result.HasValue);
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
            var result = car.TryWith(null as EmptyProps);

            // Assert
            Assert.False(result.HasValue);
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
            var result = car.TryWith(new { Color = "Blue" });

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(car.Name, result.Value.Name);
            Assert.Equal("Blue", result.Value.Color);
            Assert.NotSame(car, result);
        }
    }
}
