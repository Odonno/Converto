using Xunit;

namespace Converto.UnitTests
{
    public class IsDeepEqualTests
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
            bool result = car1.IsDeepEqual(car2);

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
            bool result = car1.IsDeepEqual(car2);

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
            bool result = car1.IsDeepEqual(car2);

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
            bool result = car1.IsDeepEqual(car2);

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
            bool result = car1.IsDeepEqual(car2);

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
            bool result = car1.IsDeepEqual(car2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoIdenticalStrings_Returns_True()
        {
            // Arrange
            string str1 = "alpha";
            string str2 = "alpha";

            // Act
            bool result = str1.IsDeepEqual(str2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoDifferentStrings_Returns_False()
        {
            // Arrange
            string str1 = "alpha";
            string str2 = "beta";

            // Act
            bool result = str1.IsDeepEqual(str2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoAnonymousTypesWithSameProperties_Returns_True()
        {
            // Arrange
            var car1 = new
            {
                Name = "Name of the car",
                Color = "Red"
            };
            var car2 = new
            {
                Name = "Name of the car",
                Color = "Red"
            };

            // Act
            bool result = car1.IsDeepEqual(car2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoAnonymousTypesWithDifferentProperties_Returns_False()
        {
            // Arrange
            var car1 = new
            {
                Name = "Name of the car",
                Color = "Red"
            };
            var car2 = new
            {
                Name = "Name of the car",
                Color = "Blue"
            };

            // Act
            bool result = car1.IsDeepEqual(car2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoBoxedAndIdenticalStrings_Returns_True()
        {
            // Arrange
            string str1 = "alpha";
            string str2 = "alpha";

            object o1 = (object)str1;
            object o2 = (object)str2;

            // Act
            bool result = o1.IsDeepEqual(o2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoBoxedAndDifferentStrings_Returns_False()
        {
            // Arrange
            string str1 = "alpha";
            string str2 = "beta";

            object o1 = (object)str1;
            object o2 = (object)str2;

            // Act
            bool result = o1.IsDeepEqual(o2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TwoUnboxedAndIdenticalStrings_Returns_True()
        {
            // Arrange
            object o1 = "alpha";
            object o2 = "alpha";

            string str1 = (string)o1;
            string str2 = (string)o2;

            // Act
            bool result = str1.IsDeepEqual(str2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TwoUnboxedAndDifferentStrings_Returns_False()
        {
            // Arrange
            object o1 = "alpha";
            object o2 = "beta";

            string str1 = (string)o1;
            string str2 = (string)o2;

            // Act
            bool result = str1.IsDeepEqual(str2);

            // Assert
            Assert.False(result);
        }
    }
}
