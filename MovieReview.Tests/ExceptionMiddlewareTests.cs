using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MovieReview.Exceptions;
using MovieReview.Middleware;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ExceptionMiddlewareTests
    {
        private Mock<ILogger<ExceptionMiddleware>> _loggerMock = null!;
        private Mock<IHostEnvironment> _envMock = null!;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            _envMock = new Mock<IHostEnvironment>();
        }

        [Test]
        public async Task InvokeAsync_NoException_CallsNext()
        {
            var nextCalled = false;
            RequestDelegate next = (ctx) => { nextCalled = true; return Task.CompletedTask; };
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context);

            Assert.That(nextCalled, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
        {
            RequestDelegate next = (ctx) => throw new NotFoundException("Not found");
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task InvokeAsync_UnauthorizedException_ReturnsUnauthorized()
        {
            RequestDelegate next = (ctx) => throw new UnauthorizedException("Unauthorized");
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task InvokeAsync_UnauthorizedAccessException_ReturnsForbidden()
        {
            RequestDelegate next = (ctx) => throw new UnauthorizedAccessException("Forbidden");
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task InvokeAsync_ArgumentException_ReturnsBadRequest()
        {
            RequestDelegate next = (ctx) => throw new ArgumentException("Bad request");
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task InvokeAsync_GenericException_ReturnsInternalServerError()
        {
            RequestDelegate next = (ctx) => throw new Exception("Boom");
            var middleware = new ExceptionMiddleware(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}
