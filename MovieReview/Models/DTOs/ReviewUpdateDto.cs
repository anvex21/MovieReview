namespace MovieReview.Models.DTOs
{
    public class ReviewUpdateDto
    {
        /// <summary>
        /// Comment
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Rating
        /// </summary>
        public int Rating { get; set; }
    }
}
