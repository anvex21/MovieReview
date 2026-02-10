using MovieReview.Models.DTOs;

namespace MovieReview.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieReadDto>> GetAllAsync();
        Task<MovieReadDto> GetByIdAsync(long id);
        Task<MovieReadDto> CreateAsync(MovieCreateDto dto);
        Task UpdateAsync(long id, MovieUpdateDto dto);
        Task DeleteAsync(long id);
        Task<IEnumerable<MovieReadDto>> GetAllAsync(MovieQueryDto queryParams);
        Task<IEnumerable<MovieReadDto>> GetTopRatedAsync(int count);
        Task<IEnumerable<MovieReadDto>> GetByYearAsync(int year);
    }
}
