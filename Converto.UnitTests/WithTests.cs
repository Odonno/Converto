using System.Collections.Generic;
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

        public class Road
        {
            public int SpeedLimit { get; set; }
            public List<Car> Cars { get; set; } = new List<Car>();
        }
        public class ComplexHighway
        {
            public Road LeftRoad { get; set; }
            public Road MiddleRoad { get; set; }
            public Road RightRoad { get; set; }
        }

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
            var result = car.With<Car, EmptyProps>(null);

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
            var canWith = car.TryWith<Car, EmptyProps>(null, out var result);

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
        [Fact]
        public void With_NestedObject()
        {
            // Arrange
            var highway = new ComplexHighway
            {
                LeftRoad = new Road
                {
                    SpeedLimit = 40,
                    Cars = new List<Car>
                    {
                        new Car
                        {
                            Name = "Red car",
                            Color = "Red"
                        }
                    }
                }
            };

            // Act
            var result = highway
                .With(new
                {
                    LeftRoad = highway.LeftRoad
                        .With(new
                        {
                            SpeedLimit = 70
                        })
                });

            // Assert
            Assert.Equal(70, result.LeftRoad.SpeedLimit);
            Assert.Single(result.LeftRoad.Cars);
            Assert.Equal("Red car", result.LeftRoad.Cars[0].Name);
            Assert.Equal("Red", result.LeftRoad.Cars[0].Color);
            Assert.Null(result.MiddleRoad);
            Assert.Null(result.RightRoad);
        }
    }
}
