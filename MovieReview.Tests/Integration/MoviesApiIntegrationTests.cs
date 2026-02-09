using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieReview.Models.DTOs;
using NUnit.Framework;

namespace MovieReview.Tests.Integration;

[TestFixture]
public class MoviesApiIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task Register_ReturnsOk_AndToken()
    {
        var dto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid():N}"[..20],
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "P@ssw0rd!"
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/Register", dto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<AuthResultDto>(JsonOptions);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginDto { Username = "nonexistent", Password = "wrong" };

        var response = await _client.PostAsJsonAsync("/api/Auth/Login", dto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetMovies_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/Movies/GetAllMovies");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetMovies_WithValidToken_ReturnsOk()
    {
        var (token, _) = await RegisterAndGetToken();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Movies/GetAllMovies");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var movies = await response.Content.ReadFromJsonAsync<List<MovieReadDto>>(JsonOptions);
        Assert.That(movies, Is.Not.Null);
        // If movies are returned, ensure each has required fields populated
        if (movies!.Count > 0)
        {
            Assert.That(movies, Has.All.Matches<MovieReadDto>
                (m => m.Id > 0 && !string.IsNullOrWhiteSpace(m.Title)));
        }
    }

    private async Task<(string Token, long UserId)> RegisterAndGetToken()
    {
        var dto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid():N}"[..20],
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "P@ssw0rd!"
        };
        var response = await _client.PostAsJsonAsync("/api/Auth/Register", dto);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResultDto>(JsonOptions);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Token, Is.Not.Null.And.Not.Empty);
        return (result.Token, 0);
    }
}
