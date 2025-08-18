namespace MovieReview.Models.DTOs
{
    public class ReviewCreateDto
    {
        /// <summary>
        /// Comment
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Rating
        /// </summary>
        public int Rating { get; set; }
        /// <summary>
        /// MovieId (FK)
        /// </summary>
        public long MovieId { get; set; }
    }
}
