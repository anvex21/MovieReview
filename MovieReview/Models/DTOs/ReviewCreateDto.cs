using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.DTOs
{
    public class ReviewCreateDto
    {
        /// <summary>
        /// Comment
        /// </summary>
        [Required]
        [StringLength(500)]
        public required string Content { get; set; }
        /// <summary>
        /// Rating
        /// </summary>
        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }
        /// <summary>
        /// MovieId (FK)
        /// </summary>
        [Required]
        public long MovieId { get; set; }
    }
}
