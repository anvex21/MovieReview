using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovieReview.Services
{
    public class ExternalMovieService : IExternalMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string _baseUrl;

        private class OmdbResponse
        {
            public string? imdbRating { get; set; }
            public string? Response { get; set; }
        }

        public ExternalMovieService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Read configuration
            _apiKey = configuration["Omdb:ApiKey"];
            _baseUrl = configuration["Omdb:BaseUrl"] ?? "https://www.omdbapi.com/";
        }

        public async Task<string?> GetImdbRatingAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "N/A";
            }

            // If API key is not configured, fail gracefully without doing an HTTP call.
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return "N/A";
            }

            try
            {
                var url = $"{_baseUrl}?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
                var response = await _httpClient.GetFromJsonAsync<OmdbResponse>(url);

                // OMDb returns { Response: "False", Error: "Movie not found!" } when not found
                if (response == null || string.Equals(response.Response, "False", StringComparison.OrdinalIgnoreCase))
                {
                    return "N/A";
                }

                return !string.IsNullOrWhiteSpace(response.imdbRating) ? response.imdbRating : "N/A";
            }
            catch
            {
                // Do not let external API issues break our API.
                return "N/A";
            }
        }
    }
}

