using NUnit.Framework;
using MovieReview.Exceptions;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ExceptionsTests
    {
        [Test]
        public void NotFoundException_SetsMessage()
        {
            var message = "Resource not found";
            var exception = new NotFoundException(message);

            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void UnauthorizedException_SetsMessage()
        {
            var message = "Invalid credentials";
            var exception = new UnauthorizedException(message);

            Assert.That(exception.Message, Is.EqualTo(message));
        }
    }
}
