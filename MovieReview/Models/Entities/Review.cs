using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.Entities
{
    public class Review
    {
        /// <summary>
        /// Review Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Review Content
        /// </summary>
        [Required, MaxLength(2000)]
        public string Content { get; set; }

        /// <summary>
        /// Rating (1-10 for example)
        /// </summary>
        [Range(1, 10)]
        public int Rating { get; set; }

        /// <summary>
        /// MovieId (FK)
        /// </summary>
        public long MovieId { get; set; }

        /// <summary>
        /// Navigation property (MOVIE)
        /// </summary>
        public Movie Movie { get; set; }

        /// <summary>
        /// UserId (FK)
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Navigation property (USER)
        /// </summary>
        public User User { get; set; }
    }
}
