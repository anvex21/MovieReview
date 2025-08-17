using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.Entities
{
    public class Movie
    {
        /// <summary>
        /// Movie ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Movie title
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Release Year
        /// </summary>
        public int ReleaseYear { get; set; }

        /// <summary>
        /// Reviews
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// Ratings
        /// </summary>
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
