using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.DTOs
{
    public class ReviewUpdateDto
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
    }
}
