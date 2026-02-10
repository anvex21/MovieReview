using Moq;
using Moq.Protected;
using NUnit.Framework;
using MovieReview.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ExternalMovieServiceTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
        private Mock<IConfiguration> _configMock = null!;
        private ExternalMovieService _service = null!;

        [SetUp]
        public void Setup()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(c => c["Omdb:ApiKey"]).Returns("test-api-key");
            _configMock.Setup(c => c["Omdb:BaseUrl"]).Returns("https://www.omdbapi.com/");
        }

        private void SetupHttpClient(string responseContent)
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _service = new ExternalMovieService(httpClient, _configMock.Object);
        }

        [Test]
        public async Task GetImdbRatingAsync_ReturnsRating_WhenMovieFound()
        {
            SetupHttpClient("{\"imdbRating\":\"8.5\",\"Response\":\"True\"}");

            var result = await _service.GetImdbRatingAsync("Inception");

            Assert.That(result, Is.EqualTo("8.5"));
        }

        [Test]
        public async Task GetImdbRatingAsync_ReturnsNA_WhenMovieNotFound()
        {
            SetupHttpClient("{\"Response\":\"False\",\"Error\":\"Movie not found!\"}");

            var result = await _service.GetImdbRatingAsync("UnknownMovie");

            Assert.That(result, Is.EqualTo("N/A"));
        }

        [Test]
        public async Task GetImdbRatingAsync_ReturnsNA_WhenApiFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _service = new ExternalMovieService(httpClient, _configMock.Object);

            var result = await _service.GetImdbRatingAsync("Inception");

            Assert.That(result, Is.EqualTo("N/A"));
        }

        [Test]
        public async Task GetImdbRatingAsync_ReturnsNA_WhenApiKeyMissing()
        {
            _configMock.Setup(c => c["Omdb:ApiKey"]).Returns((string?)null);
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _service = new ExternalMovieService(httpClient, _configMock.Object);

            var result = await _service.GetImdbRatingAsync("Inception");

            Assert.That(result, Is.EqualTo("N/A"));
        }

        [Test]
        public async Task GetImdbRatingAsync_ReturnsNA_WhenTitleEmpty()
        {
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _service = new ExternalMovieService(httpClient, _configMock.Object);

            var result = await _service.GetImdbRatingAsync("");

            Assert.That(result, Is.EqualTo("N/A"));
        }
    }
}
