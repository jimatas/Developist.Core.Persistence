using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class ArgumentExceptionHelperTests
    {
        [TestMethod]
        public void ThrowIfNull_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            object? nullValue = null;

            // Act
            void action() => ArgumentNullExceptionHelper.ThrowIfNull(() => nullValue);

            // Assert
            ArgumentException exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            string? nullValue = null;

            // Act
            void action() => ArgumentExceptionHelper.ThrowIfNullOrEmpty(() => nullValue);

            // Assert
            ArgumentException exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            string? nullValue = null;

            // Act
            void action() => ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => nullValue);

            // Assert
            ArgumentException exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual(nameof(nullValue), exception.ParamName);
        }

        [TestMethod]
        public void ThrowIfNull_GivenEmptyString_DoesNotThrow()
        {
            // Arrange
            string? emptyValue = string.Empty;

            // Act
            ArgumentNullExceptionHelper.ThrowIfNull(() => emptyValue);

            // Assert
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_GivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            string? emptyValue = string.Empty;

            // Act
            void action() => ArgumentExceptionHelper.ThrowIfNullOrEmpty(() => emptyValue);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_GivenAllWhiteSpaceString_DoesNotThrow()
        {
            // Arrange
            string? whiteSpaceValue = "  ";

            // Act
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(() => whiteSpaceValue);

            // Assert
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_GivenAllWhiteSpaceString_ThrowsArgumentException()
        {
            // Arrange
            string? whiteSpaceValue = "  ";

            // Act
            void action() => ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => whiteSpaceValue);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void ThrowIfNull_GivenSomeObject_ReturnsIt()
        {
            // Arrange
            object? someObject = new();

            // Act
            var returnValue = ArgumentNullExceptionHelper.ThrowIfNull(() => someObject);

            // Assert
            Assert.AreEqual(someObject, returnValue);
        }

        [TestMethod]
        public void ThrowIfNullOrEmpty_GivenValidString_ReturnsIt()
        {
            // Arrange
            string? value = "Has a value";

            // Act
            var returnValue = ArgumentExceptionHelper.ThrowIfNullOrEmpty(() => value);

            // Assert
            Assert.AreEqual(value, returnValue);
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_GivenValidString_ReturnsIt()
        {
            // Arrange
            string? value = "Has a value";

            // Act
            var returnValue = ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => value);

            // Assert
            Assert.AreEqual(value, returnValue);
        }

        [TestMethod]
        public void ThrowIfOutOfRange_GivenValueLessThanMinValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int minValue = 0;
            int value = -1;

            // Act
            void action() => ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => value, minValue);

            // Assert
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
            Assert.IsTrue(exception.Message.StartsWith($"Value cannot be less than {minValue}."));
        }

        [TestMethod]
        public void ThrowIfOutOfRange_GivenValueGreaterThanMaxValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int maxValue = 1;
            int value = 2;

            // Act
            void action() => ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => value, maxValue: maxValue);

            // Assert
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
            Assert.IsTrue(exception.Message.StartsWith($"Value cannot be greater than {maxValue}."));
        }

        [TestMethod]
        public void ThrowIfOutOfRange_GivenInvalidValueWithBothBoundsSpecified_ThrowsWithExpectedMessage()
        {
            // Arrange
            int minValue = 0;
            int maxValue = 1;
            int value = 2;

            // Act
            void action() => ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => value, minValue, maxValue);

            // Assert
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
            Assert.IsTrue(exception.Message.StartsWith($"Value cannot be less than {minValue} or greater than {maxValue}."));
        }

        [TestMethod]
        public void ThrowIfOutOfRange_GivenValidValue_ReturnsIt()
        {
            // Arrange
            int minValue = 0;
            int maxValue = 1;
            int value = 1;

            // Act
            var returnValue = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => value, minValue, maxValue);

            // Assert
            Assert.AreEqual(value, returnValue);
        }
    }
}
