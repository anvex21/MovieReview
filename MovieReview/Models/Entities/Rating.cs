using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.Entities
{
    public class Rating
    {
        /// <summary>
        /// Rating Id
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// Value
        /// </summary>
        [Range(1, 10)]
        public int Value { get; set; }

        /// <summary>
        /// Movie Id (FK)
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
