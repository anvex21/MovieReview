using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.DTOs
{
    public class MovieUpdateDto
    {
        /// <summary>
        /// Title
        /// </summary>
        [Required, MaxLength(100)]
        public required string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [Required, MaxLength(500)]
        public required string Description { get; set; }

        /// <summary>
        /// Release year
        /// </summary>
        [Required, Range(1800, 2025)]
        public int ReleaseYear { get; set; }
    }
}
