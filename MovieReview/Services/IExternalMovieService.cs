using System.Threading.Tasks;

namespace MovieReview.Services
{
    public interface IExternalMovieService
    {
        Task<string?> GetImdbRatingAsync(string title);
    }
}

