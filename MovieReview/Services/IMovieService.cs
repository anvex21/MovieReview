using MovieReview.Models.DTOs;

namespace MovieReview.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieReadDto>> GetAllAsync();
        Task<MovieReadDto> GetByIdAsync(long id);
        Task<MovieReadDto> CreateAsync(MovieCreateDto dto);
        Task<bool> UpdateAsync(long id, MovieUpdateDto dto);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<MovieReadDto>> GetAllAsync(MovieQueryDto queryParams);
    }
}
